# PowerShell equivalent script
# Ensure script stops if any command fails
$ErrorActionPreference = "Stop"

Write-Host "Hello world"

if (-not (Get-Command choco -ErrorAction SilentlyContinue)) {
    Set-ExecutionPolicy Bypass -Scope Process -Force
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
    Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
}

choco upgrade chocolatey -y

choco install python --version=3.11.0 -y

python --version

python -m venv venv
.\venv\Scripts\activate

python -m pip install --upgrade pip
pip install setuptools wheel future absl-py "numpy<2" jax[cpu] opencv-contrib-python protobuf==3.20.1 six==1.14.0 tensorflow tf_slim

choco install git wget cmake curl ffmpeg -y

choco install bazelisk -y

choco install llvm -y

choco install nodejs -y
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

Copy-Item -Force ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dll ../../mediapipe_language_detector.dll

Set-Location ../..
Write-Host "Goodbye world"
