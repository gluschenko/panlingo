# PowerShell equivalent script
# Ensure script stops if any command fails
$ErrorActionPreference = "Stop"

Write-Output "Hello world"

$workspace = "obj/native_build_temp"

# Copy directories
Copy-Item -Path "Native\lingua-ffi\." -Destination $workspace -Recurse -Force

# List directory contents recursively
Get-ChildItem -Recurse -Path .

Set-Location $workspace

# Build for Windows
cargo build --release

# List directory contents recursively
Get-ChildItem -Recurse -Path .

# Display shared library dependencies
Copy-Item -Path ".\target\release\lingua.dll" -Destination "..\..\lingua.dll"

# Clean up
Write-Output "Goodbye world"