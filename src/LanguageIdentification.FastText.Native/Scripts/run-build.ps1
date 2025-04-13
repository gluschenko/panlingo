# PowerShell equivalent script
# Ensure script stops if any command fails
$ErrorActionPreference = "Stop"

Write-Output "Hello world"

$workspace = "obj/native_build_temp"

# Create directory if it doesn't exist
if (-Not (Test-Path $workspace)) {
    New-Item -Path $workspace -ItemType Directory
    New-Item -Path "$workspace/fasttext" -ItemType Directory
}

# Copy directories
Copy-Item -Path "../../third_party/fastText/*" -Destination "$workspace/fasttext" -Recurse -Force
Copy-Item -Path "Native/*" -Destination $workspace -Recurse -Force

# List directory contents recursively
Get-ChildItem -Recurse -Path .

Set-Location $workspace

# Create and enter build directory
if (-Not (Test-Path "build")) {
    New-Item -Path "build" -ItemType Directory
}
Set-Location "build"

# Build for Windows
cmake ..
cmake --build .

# List directory contents recursively
Get-ChildItem -Recurse -Path .

# Display shared library dependencies
Copy-Item -Path ".\Debug\fasttext.dll" -Destination "..\..\fasttext.dll"

# List directory contents recursively
Get-ChildItem -Recurse -Path .

# Clean up
cd ../..
Write-Output "Goodbye world"