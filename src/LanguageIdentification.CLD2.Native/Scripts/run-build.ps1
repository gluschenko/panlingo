
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Hello world"

$workspace = "obj/native_build_temp"

# Create directory if it doesn't exist
if (-Not (Test-Path $workspace)) {
    New-Item -Path $workspace -ItemType Directory
    New-Item -Path "$workspace/cld2" -ItemType Directory
}

Copy-Item -Path "../../third_party/cld2/*" -Destination "$workspace/cld2" -Recurse
Copy-Item -Path "Native/*" -Destination "$workspace" -Recurse

Get-ChildItem -Recurse -Path .

Set-Location -Path "$workspace"

New-Item -ItemType Directory -Path "build"
Set-Location -Path "build"

Remove-Item -Path "*" -Recurse -Force
cmake ..
make -j (Get-CimInstance Win32_Processor | Measure-Object -Property NumberOfCores -Sum).Sum

Get-ChildItem -Recurse

Copy-Item -Path "libcld2.so" -Destination "../../../libcld2.so"

# Clean up
Remove-Item -Path "$workspace" -Recurse -Force
Write-Host "Goodbye world"
