#!/bin/bash

# Practice safe scripting
set -e          # If a command fails, make the whole script exit,
set -u          # Treat unset variables as an error, and immediately exit.
set -f          # Disable filename expansion (globbing) upon seeing *, ?, etc..
set -o pipefail # If a command in a pipeline fails, make the whole command fail

# Source the CICD helpers to let us work with the pipeline json file
# shellcheck disable=SC1091
source ./jsonhelpers.sh;

# --acl bucket-owner-full-control is set within this method.
upload_all_stage_files_as_zip;

# Get oxford algorithm version
algorithm_version=$(jq -r '.targets.".NETCoreApp,Version=v3.1/linux-x64"[].dependencies | select(.QCovidRiskCalculator != null).QCovidRiskCalculator' deployartifact/NHSD.RiskStratification.Calculator.deps.json);

echo '=========== ALGORITHM VERSION ===========';
echo "${algorithm_version}";
echo '=========================================';

$(add_component_fact_to_json "algorithm_version" ${algorithm_version})

echo_json_file;
