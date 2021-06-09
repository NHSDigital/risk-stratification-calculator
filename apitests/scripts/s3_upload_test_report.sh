#!/usr/bin/env bash

# Practice safe scripting
set -e          # If a command fails, make the whole script exit,
set -u          # Treat unset variables as an error, and immediately exit.
set -f          # Disable filename expansion (globbing) upon seeing *, ?, etc..
set -o pipefail # If a command in a pipeline fails, make the whole command fail

# shellcheck disable=SC1091
source ../jsonhelpers.sh
source ../pipelinehelpers.sh

declare testType="$1"
declare pointVersion="$2"
declare branchAndVersion="$3"
declare jobId="$4"

environment="${PIPELINE_ENVIRONMENT:-}"
if [ ! -z "${environment}" ]; then
  if [[ "${environment}" =~ "dynamic" ]]; then
    environment="${DYNAMIC_ENVIRONMENT}_"
  else
    environment="${PIPELINE_ENVIRONMENT:-}_"
  fi
fi

declare destinationUrlPrefix="component/apitests/${environment}"
declare website="${REPORTS_S3_WEBROOT}${destinationUrlPrefix}${branchAndVersion}_${jobId}/${testType}/index.html"

. scripts/s3_upload_folder.sh  "reports/html/" "${testType}/" "${destinationUrlPrefix}" "${branchAndVersion}" "${jobId}" "${REPORTS_S3_BUCKET_NAME}" "${REPORTS_S3_WEBROOT}" "${REPORTS_S3_CLOUDFRONT_ID}"

# Write test website url to metadata file
mkdir -p reports/build-data/
cat > "reports/build-data/artifact_metadata_fragment_${testType}.json" <<EOF
{
  "test_links": {
    "${testType}": "${website}"
  }
}
EOF

echo "================================================================="
pwd
ls -l "reports/build-data/artifact_metadata_fragment_${testType}.json"
echo "View website: $website"
echo "================================================================="

# read metadata files written by s3_upload_test_report.sh
set +f # allow globbing
echo -e "merging test_links from: \n$(ls -1 reports/build-data/artifact_metadata_fragment_*.json || true)"
declare metadata test_reports_urls="{}" matadata_file_count
matadata_file_count="$(find reports/build-data -name 'artifact_metadata_fragment_*.json' | wc -l)"
echo "Found ${matadata_file_count} metadata fragment files"
if [[ "${matadata_file_count}" != "0" ]]; then
    metadata="$(cat reports/build-data/artifact_metadata_fragment_*.json | jq --slurp 'reduce .[] as $item ({}; . * $item)')"
    test_reports_urls="$(echo "${metadata}" | jq .test_links)"
fi

echo test_reports_urls:
echo ${test_reports_urls}
echo

# add the test report links to the artifact metadata
cd ..
temp_json=$(jq --indent 2 ".components.apitests.artifact_metadata.test_links = ${test_reports_urls}" "$(get_environment_json)");
echo "${temp_json}" | jq . > "$(get_environment_json)";
echo_json_file;
