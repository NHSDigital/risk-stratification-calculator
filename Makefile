SHELL=/bin/bash	-euo pipefail

ifndef CI
EXECUTABLES = dotnet sam zip yarn jq
K := $(foreach exec,$(EXECUTABLES), $(if $(shell $(exec) --version || echo $?),some string,$(error "Unable to check $(exec) --version in PATH. Please install.")))

DOTNET_TOOLS = amazon.lambda.tools dotnet-reportgenerator-globaltool
L := $(foreach tool,$(DOTNET_TOOLS), $(if $(shell dotnet tool list -g | grep $(tool) || echo $?),some string,$(error "Unable to find dotnet tool $(tool). Please install with 'dotnet tool install -g $(tool).")))
endif

PATCH_VERSION := $(if $(CI_PIPELINE_IID),$(CI_PIPELINE_IID),0)

MEDICAL_CODES_VERSION := 1.0.1
TOWNSEND_DATA_VERSION := 1.0.1

TOWNSEND_AWS_DEV_BUCKET = s3-bucket
TOWNSEND_AWS_LOCATION = townsend

## If we're running in CI and the index files already exist in prod we've already deployed them to all environments
TOWNSEND_SHOULD_CONVERT := $(if $(CI_PIPELINE_IID),$(shell aws s3 ls s3://$(TOWNSEND_AWS_PROD_BUCKET)/FF || echo 0),0)

.setup-dotnet:
	dotnet restore *.sln

.setup-yarn:
	yarn --cwd apitests install
	INIT_CWD=apitests npm_config_user_agent="$(shell yarn config get user-agent)" node apitests/node_modules/husky/husky.js install

.clean:
	rm -rf */bin */obj

.build: .clean
	dotnet build -c	Release	--version-suffix $(PATCH_VERSION) *.sln

.lint:
	yarn --cwd apitests run lint

.test:
	dotnet test

.test-reports:
	./execute-test-reporting.sh

.test-coverage:
	rm -rf */TestResults reports
	dotnet test --logger:trx --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[NHSD.RiskStratification.Calculator.Tests]*

.allure-report:
	yarn --cwd apitests/ run allure	generate --clean -o ../reports/allure-reports/NHSD.RiskStratification.Calculator.Tests '../NHSD.RiskStratification.Calculator.Tests/TestResults'

.coverage-report:
	~/.dotnet/tools/reportgenerator -reports:*/TestResults/*/coverage.cobertura.xml -targetdir:reports/coverage-report -reporttypes:"Html;Cobertura;TextSummary"
	cat	reports/coverage-report/Summary.txt

.get-version:
	$(eval VERSION := $(shell cat Directory.Build.props | grep -P "(?<=<VersionPrefix>).*(?=</VersionPrefix>)" -o))
	$(eval POINT_VERSION=$(VERSION).$(PATCH_VERSION))
	$(eval BRANCH_AND_VERSION=$(CI_COMMIT_REF_NAME)_$(POINT_VERSION))

.package: .clean
	./NHSD.RiskStratification.Calculator/package.sh "$(PATCH_VERSION)" "$(POINT_VERSION)"

.package-apitests:
	./apitests/package.sh "$(PATCH_VERSION)" "$(POINT_VERSION)"

.sam-local:
	sam local start-api --debug \
	 --template NHSD.RiskStratification.Calculator/template.yaml \
     --parameter-overrides "ParameterKey=QCovidLicenceKey,ParameterValue=${QCovidLicenceKey} ParameterKey=RISKSTRAT_QCovidTownsendIndex__Region,ParameterValue=${RISKSTRAT_QCovidTownsendIndex__Region} ParameterKey=RISKSTRAT_QCovidTownsendIndex__Bucket,ParameterValue=${RISKSTRAT_QCovidTownsendIndex__Bucket} ParameterKey=RISKSTRAT_QCovidTownsendIndex__Path,ParameterValue=${RISKSTRAT_QCovidTownsendIndex__Path}"

.test-integration:
	yarn --cwd apitests run concurrently "make --directory .. start" "wait-on tcp:3000 && yarn run test:integration" --kill-others --success first

.cli-authentication-reminder:
ifndef CI
	@echo "==============================================="
	@echo "Ensure you are authenticated with your AWS cli!"
	@echo "==============================================="
endif

.download-townsend-data: .cli-authentication-reminder 
	aws	s3 cp --recursive --exclude "*" --include "townsend*.zip" s3://$(TOWNSEND_AWS_LOCATION)/$(TOWNSEND_DATA_VERSION)/ tmp
	unzip -d tmp/townsend-data tmp/townsend*.zip
	mv tmp/townsend-data/*.sql tmp/townsend-data/townsend-data.sql

.download-medical-lookup-data: .cli-authentication-reminder 
	aws	s3 cp --recursive --exclude "*" --include "medical-codes*.sqlite" s3://$(TOWNSEND_AWS_LOCATION)/$(TOWNSEND_DATA_VERSION)/ tmp
	mv tmp/medical-codes* tmp/medical-lookup.sqlite

.cleanup-temp-data:
	rm -rf tmp/townsend-data.zip tmp/townsend-data tmp/medical-lookup.sqlite tmp

.build-docker-image:
	docker build -t riskstrat-batch:local -f NHSD.RiskStratification.Calculator.Batch/Dockerfile .

.build-docker: .download-medical-lookup-data .build-docker-image .cleanup-temp-data

# Build TownsendIndexConverter docker file, locally.
.build-docker-townsend-converter: .cli-authentication-reminder .download-townsend-data
	docker rmi -f townsend-converter-$(TOWNSEND_DATA_VERSION):local
	docker build -t townsend-converter-$(TOWNSEND_DATA_VERSION):local -f NHSD.RiskStratification.Calculator.TownsendIndexConverter/Dockerfile .
	make .cleanup-temp-data

# Run the TownsendIndexConverter process and copy out the results from the docker container on the te host machine.
.run-docker-townsend-converter: .cleanup-docker-townsend-converter
	mkdir townsend
	docker run --name townsend_index_converter townsend-converter-$(TOWNSEND_DATA_VERSION):local
	docker cp townsend_index_converter:/app/binary townsend

.cleanup-docker-townsend-converter:
	docker container rm townsend_index_converter || true
	rm -rf townsend

.convert-townsend-data: .cleanup-temp-data
ifeq ($(TOWNSEND_SHOULD_CONVERT), 0)
	make .build-docker-townsend-converter
	make .run-docker-townsend-converter
else
	@echo "===================================================="
	@echo "Ignoring version [$(TOWNSEND_DATA_VERSION)] of townsend data is deployed in bucket [$(TOWNSEND_AWS_PROD_BUCKET)]."
	@echo "===================================================="
endif

help:
	@echo "Commands"
	@echo "========"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2, $$3, $$4, $$5, $$6, $$7, $$8, $$9, $$10, $$11, $$12, $$13, $$14, $$15, $$16, $$17, $$18, $$19, $$20}'

# main scripts
setup: .setup-dotnet .setup-yarn ## Restores dotnet packages and yarn and inform about the aws helpers

build: .build ## Compiles the project

build-docker: .build-docker ## Builds :local tagged projects with docker support

lint: .lint ## Lints JS codebase using eslint

test: build .test ## Executes unit tests

test-integration: .test-integration ## Runs JS integration tests against AWS SAM hosted API

test-coverage: build .test-reports ## Executes unit tests with code coverage and generates html report

package: .get-version .package ## Creates the lambda package

package-apitests: .get-version .package-apitests ## Creates the apitests package

start: package .sam-local ## Starts API locally using AWS SAM

build-townsend-data: .convert-townsend-data ## Will download the townsend.sql from AWS and create binary files of the .sql data the zip them up.
