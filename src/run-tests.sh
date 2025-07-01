#!/bin/bash

image_name="panlingo-test-image"
container_name="panlingo-test-runner"

dotnet build -c ReleaseLinuxOnly || exit 1

docker build --file test.Dockerfile -t "${image_name}" . || exit 1
docker container create --name "${container_name}" -v "$PWD:/src" -i "${image_name}" || exit 1
docker container start "${container_name}" || exit 1
docker exec "${container_name}" sh -c "cd /src && dotnet test -c ReleaseLinuxOnly -l 'console;verbosity=detailed'"

read -n1 -rsp $'Press any key to continue...\n'

docker rm "${container_name}" --force
