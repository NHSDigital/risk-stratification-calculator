SHELL=/bin/bash	-euo pipefail

PATCH_VERSION := $(if $(CI_PIPELINE_IID),$(CI_PIPELINE_IID),0)

.setup:
	yarn install --immutable
	INIT_CWD=$(shell pwd) npm_config_user_agent="$(shell yarn config get user-agent)" node node_modules/husky/husky.js install

.test-integration:
	yarn run test:integration

.test-allure-generate: .get-version
	yarn run test:allure:generate	

setup: .setup

test-integration: .test-integration

test-allure-generate: .test-allure-generate