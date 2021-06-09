# NHSD Risk Stratification Calculator

This is a .Net Core 3.1 project containing the Lambda-hosted API functionality for the Risk Stratification Service.

The tooling for this project can be executed via the `make` command using referencing the Makefile found at the root of this project.
The following tools are required to be installed locally in order to fully utilise the commands within this Makefile:

- Dotnet Core 3.1 SDK
- [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) (installable with `dotnet tool install -g Amazon.Lambda.Tools`)
- AWS Serverless Application Model (https://aws.amazon.com/serverless/sam/)
- Docker

**Please Note:** This project will not build without the correct dll in the correct place, please see the [readme.txt](lib/OxfordQCovid/readme.txt).

## Setup
- If you're on a Mac you'll need to update your `.bashrc` file with the following lines:
    ```
    export QCovidLicenceKey=ASK_CHANNEL
    export RISKSTRAT_QCovidTownsendIndex__Region=eu-west-2
    export RISKSTRAT_QCovidTownsendIndex__Bucket=<TOWNSEND_AWS_DEV_BUCKET from the Makefile>
    export RISKSTRAT_QCovidTownsendIndex__Path=<TOWNSEND_DATA_VERSION from the Makefile>
    ```
- If you're on Windows you can set a windows environment variable and pass it down to WSL [Read here](https://devblogs.microsoft.com/commandline/share-environment-vars-between-wsl-and-windows/)

## Updating the OCC QCovid.RiskCalculator library
### OCC Version checks
There are 3 checks within the OCC codebase that can change from version to version:
#### Licence Key
When running locally and for tests, this is done via an environment variable defined within your ~/.bashrc file as defined earlier in this file.

#### Townsend Index
The Townsend Index is a hashed dataset mapping postcodes to townsend scores.
Within this dataset, there is a specific entry that matches the value 'VERSION' in place of the postcode.

#### Medical code lookup
The medical code index will automatically be pulled down and embedded into the batch docker container on building.
However to support automated testing, the bare bones of an sqlite database are created in memory when running tests.
As part of this, there is a Versions table that is defined with a single row, that will need to be updated based on the
matching contents within the medical-codes.sqlite database provided within the release. If these versions do not match
an error will be thrown and the tests will fail.

## Make setup
This will execute a dotnet restore of the project, pulling down the related nuget dependencies

## Make build
This will compile the project in a release configuration

## Make build-townsend-data
This will locally create the townsend binary index files using source data from a predefined s3 bucket and data version

## Make package
This will compile the project into the lambda executable zip within NHSD.RiskStratification.Calculator/bin/release/netcoreapp3.1 using the dotnet-lambda tool

## Make test
This will execute the unit tests for the NHSD.RiskStratification.Calculator project

## Make start
This will package and then deploy the lambda executable package using the AWS SAM toolset for local execution
To support execution, you will need to ensure that the release folder 'NHSD.RiskStratification.Calculator/bin/Release/netcoreapp3.1/publish/` is a shared drive.

## Make test-integration
This will package will run any integration api tests under apitests folder

To facilitate testing, an example request can be found within 'sample-request.json'.
Copy the contents of `sample-request.json` and paste it into Postman as a `POST` request to `http://localhost:3000/api/qcovid`

## Updating Approval tests
We're using approval tests to validate that the logger is called and verifying the contents of that log message for each Lambda tests (think snapshot tests in JS world).


We're using the `[UseReporter(typeof(DiffReporter))]` this will bring up a diff window to explore the differences between the recieved file and the approved files. You can use this diff explorer to accept any changes and the `*.approved.json` will be updated or created.
