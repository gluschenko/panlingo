﻿$image_name = "panlingo-test-image"
$container_name = "panlingo-test-runner"

dotnet build -c ReleaseLinuxOnly

docker build --file test.Dockerfile -t ${image_name} .
docker container create --name ${container_name} -v "${PWD}:/src" -i ${image_name}
docker container start ${container_name}
docker exec ${container_name} sh -c "cd /src && dotnet test -c ReleaseLinuxOnly -l 'console;verbosity=detailed'"

Write-Host -NoNewLine 'Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

docker rm ${container_name} --force
