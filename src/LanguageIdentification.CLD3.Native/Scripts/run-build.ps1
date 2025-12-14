
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

param(
    [ValidateSet("x86_64", "arm64")]
    [string]$ARCH
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not $ARCH) {
    throw "ARCH argument is required. Usage: ./run-build.ps1 <x86_64|arm64>"
}

Write-Host "Hello world $ARCH"

switch ($ARCH) {
    "x86_64" { $CMakeArch = "x64" }
    "arm64"  { $CMakeArch = "ARM64" }
}

$workspace = "obj/native_build_temp"

# Create directory if it doesn't exist
if (-Not (Test-Path $workspace)) {
    New-Item -Path $workspace -ItemType Directory
    New-Item -Path "$workspace/cld3" -ItemType Directory
}

Copy-Item -Path "../../third_party/cld3/*" -Destination "$workspace/" -Recurse
Copy-Item -Path "Native/*" -Destination $workspace -Recurse -Force

# List directory contents recursively
Get-ChildItem -Recurse -Path .

Set-Location $workspace

npm install -g zx
zx ./monkey-patch.mjs

# Create and enter build directory
if (-Not (Test-Path "build")) {
    New-Item -Path "build" -ItemType Directory
}
Set-Location "build"

Remove-Item -Path "*" -Recurse -Force
# Build for Windows
cmake .. -A $CMakeArch
cmake --build .

Get-ChildItem -Recurse

Copy-Item -Path ".\Debug\cld3.dll" -Destination "..\..\..\libcld3.$ARCH.dll"

cd ../..
Write-Output "Goodbye world"
