
#!/bin/bash

# Practice safe scripting
set -e          # If a command fails, make the whole script exit,
set -u          # Treat unset variables as an error, and immediately exit.
set -o pipefail # If a command in a pipeline fails, make the whole command fail

PATCH_VERSION="$1"
POINT_VERSION="$2"

ZIP_PATH="NHSD.RiskStratification.Calculator/bin/Release/netcoreapp3.1"
ZIP_FILE="${ZIP_PATH}/NHSD.RiskStratification.Calculator-${POINT_VERSION}.zip"
~/.dotnet/tools/dotnet-lambda package -pl NHSD.RiskStratification.Calculator --msbuild-parameters "--version-suffix $PATCH_VERSION" -o "$ZIP_FILE"

echo  "generating artifact metadata"

commit_ref="${CI_COMMIT_REF_NAME:-}"
commit_sha="${CI_COMMIT_SHA:-}"

metadata={}
metadata=$(echo $metadata | jq --arg git_commit_ref "$commit_ref" '. += {git_commit_ref:$git_commit_ref}')
metadata=$(echo $metadata | jq --arg git_commit_hash "$commit_sha" '. += {git_commit_hash:$git_commit_hash}')

echo "${metadata}" | jq . > "$ZIP_PATH"/artifact-metadata.json;
echo "calcengine: artifact-metadata.json: "
jq '.' "$ZIP_PATH"/artifact-metadata.json
echo "==========="

zip -rv -j "$ZIP_FILE" "$ZIP_PATH"/artifact-metadata.json
