
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Hello world"

$workspace = "obj/native_build_temp"

# Create directory if it doesn't exist
if (-Not (Test-Path $workspace)) {
    New-Item -Path $workspace -ItemType Directory
    New-Item -Path "$workspace/cld3" -ItemType Directory
}

Copy-Item -Path "../../third_party/cld3/*" -Destination "$workspace/" -Recurse
Copy-Item -Path "Native/*" -Destination $workspace -Recurse -Force

Get-ChildItem -Recurse -Path .

Set-Location -Path "$workspace"

nvm install 22
nvm use 22

npm install -g zx

zx ./monkey-patch.mjs

New-Item -ItemType Directory -Path "build"
Set-Location -Path "build"

Remove-Item -Path "*" -Recurse -Force
cmake ..

$numCores = (Get-CimInstance Win32_Processor | Measure-Object -Property NumberOfCores -Sum).Sum
make -j $numCores

.\Debug\language_identifier_main.exe
.\Debug\language_identifier_features_test.exe

Get-ChildItem -Recurse

Copy-Item -Path ".\Debug\libcld3.dll" -Destination "..\..\..\libcld3.dll"

Remove-Item -Path $workspace -Recurse -Force
Write-Host "Goodbye world"