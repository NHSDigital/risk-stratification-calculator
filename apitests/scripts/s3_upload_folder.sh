#!/usr/bin/env bash

# Practice safe scripting
set -e          # If a command fails, make the whole script exit,
set -u          # Treat unset variables as an error, and immediately exit.
set -f          # Disable filename expansion (globbing) upon seeing *, ?, etc..
set -o pipefail # If a command in a pipeline fails, make the whole command fail

declare srcFolderName="$1"
declare dstFolderName="$2"
declare destinationUrlPrefix="$3"
declare branchAndVersion="$4"
declare jobId="$5"
declare s3BucketName="$6"
declare s3WebsiteRoot="$7"
if [[ -n "${8-}" ]]; then
  declare s3Grants="--grants read=id=${8-}"
else
  declare s3Grants=""
fi

declare targetS3url="s3://${s3BucketName}/${destinationUrlPrefix}${branchAndVersion}_${jobId}/${dstFolderName}"

echo "Uploading ${srcFolderName} to ${targetS3url}"
echo "-------------------------"
ls -lhF "${srcFolderName}"
echo "-------------------------"
# ensure that --acl bucket-owner-full-control is not set here since uploading the allure reports uses this file.
time aws s3 cp --recursive --only-show-errors "${srcFolderName}"/ "${targetS3url}" ${s3Grants}