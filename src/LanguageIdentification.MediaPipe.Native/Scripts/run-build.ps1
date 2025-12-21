
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

python --version
python -m venv venv
.\venv\Scripts\activate

npm install -g zx

bazel version
node --version
npm --version
clang --version

$workspace = "obj/native_build_temp"

New-Item -ItemType Directory -Force -Path $workspace | Out-Null

Copy-Item -Recurse -Force ../../third_party/mediapipe/* $workspace/
Copy-Item -Recurse -Force ./Native/* $workspace/

Set-Location $workspace

zx ./monkey-patch.mjs

bazel build -c opt `
    --linkopt -s --strip always `
    --define MEDIAPIPE_DISABLE_GPU=1 `
    --define "absl=0" `
    --sandbox_debug --verbose_failures `
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.dll

Copy-Item -Force ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dll ../../mediapipe_language_detector.$ARCH.dll

Set-Location ../..
Write-Host "Goodbye world"
