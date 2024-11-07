# PowerShell equivalent script
# Ensure script stops if any command fails
$ErrorActionPreference = "Stop"

Write-Output "Hello world"

$workspace = "build_temp"

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
Copy-Item -Path ".\target\release\liblingua.dll" -Destination "../../../liblingua.dll"

# List directory contents recursively
Get-ChildItem -Recurse -Path .

# Clean up
cd ../..
Write-Output "Goodbye world"