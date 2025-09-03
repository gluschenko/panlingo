
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
Copy-Item -Path "Native/*" -Destination $workspace -Recurse -Force

# List directory contents recursively
Get-ChildItem -Recurse -Path .

Set-Location $workspace

# Create and enter build directory
if (-Not (Test-Path "build")) {
    New-Item -Path "build" -ItemType Directory
}
Set-Location "build"

Remove-Item -Path "*" -Recurse -Force
# Build for Windows
cmake ..
cmake --build .

Get-ChildItem -Recurse

Copy-Item -Path "cld2.dll" -Destination "../../../libcld2.dll"

# Clean up
Remove-Item -Path "$workspace" -Recurse -Force
Write-Host "Goodbye world"
