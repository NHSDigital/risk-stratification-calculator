
#!/bin/bash

# Practice safe scripting
set -e          # If a command fails, make the whole script exit,
set -u          # Treat unset variables as an error, and immediately exit.
set -o pipefail # If a command in a pipeline fails, make the whole command fail

PATCH_VERSION="$1"
POINT_VERSION="$2"

ZIP_PATH="apitests"
ZIP_FILE="${ZIP_PATH}/v${POINT_VERSION}.zip"

echo  "generating artifact metadata"

commit_ref="${CI_COMMIT_REF_NAME:-}"
commit_sha="${CI_COMMIT_SHA:-}"

metadata={}
metadata=$(echo $metadata | jq --arg version "$PATCH_VERSION" '. += {version:$version}')
metadata=$(echo $metadata | jq --arg git_commit_ref "$commit_ref" '. += {git_commit_ref:$git_commit_ref}')
metadata=$(echo $metadata | jq --arg git_commit_hash "$commit_sha" '. += {git_commit_hash:$git_commit_hash}')

rm -rf "$ZIP_PATH"/dist
mkdir "$ZIP_PATH"/dist

echo "${metadata}" | jq . > "$ZIP_PATH"/dist/artifact-metadata.json;
echo "apitests: artifact-metadata.json: "
jq '.' "$ZIP_PATH"/dist/artifact-metadata.json
echo "==========="

cp "$ZIP_PATH"/{helpers,scripts,config,__tests__} "$ZIP_PATH"/dist -r
cp "$ZIP_PATH"/{babel,commitlint,jest.integration}.config.js "$ZIP_PATH"/dist -r
cp "$ZIP_PATH"/{deploy.sh,Makefile,package.json,yarn.lock} "$ZIP_PATH"/dist -r

cd ./"$ZIP_PATH"/dist && zip -r v"$POINT_VERSION".zip ./ && cd -
