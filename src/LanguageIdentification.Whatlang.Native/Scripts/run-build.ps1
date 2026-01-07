
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

# Copy directories
Copy-Item -Path "Native\whatlang-ffi\." -Destination $workspace -Recurse -Force

# List directory contents recursively
Get-ChildItem -Recurse -Path .

Set-Location $workspace

# Build for Windows
cargo build --release

# List directory contents recursively
Get-ChildItem -Recurse -Path .

# Display shared library dependencies
Copy-Item -Path ".\target\release\whatlang.dll" -Destination "..\..\whatlang.$ARCH.dll"

# Clean up
Write-Output "Goodbye world"