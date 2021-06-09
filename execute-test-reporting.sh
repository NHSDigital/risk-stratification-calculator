#!/bin/bash

echo "Running dotnet tests with coverage"
make .test-coverage
declare TEST_EXIT_CODE=$?

echo "Generating allure test reports"
make .allure-report
REPORT_EXIT_CODE=$?

echo "Generating coverage report"
make .coverage-report
COVERAGE_EXIT_CODE=$?


if [[ "${TEST_EXIT_CODE}" == "0" ]]; then
  echo "Success: Tests passed"
else
  echo "ERROR: Tests failed"
  exit ${TEST_EXIT_CODE};
fi

if [[ "${REPORT_EXIT_CODE}" == "0" ]]; then
  echo "Success: Report generation passed"
else
  echo "ERROR: Report generation failed"
  exit ${REPORT_EXIT_CODE};
fi

if [[ "${COVERAGE_EXIT_CODE}" == "0" ]]; then
  echo "Success: Coverage generation passed"
else
  echo "ERROR: Coverage generation failed"
  exit ${COVERAGE_EXIT_CODE};
fi
