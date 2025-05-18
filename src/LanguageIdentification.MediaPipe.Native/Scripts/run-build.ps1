# PowerShell script for Windows
$ErrorActionPreference = "Stop"

Write-Host "Hello world"

$workspace = "obj/native_build_temp"

New-Item -ItemType Directory -Force -Path $workspace | Out-Null

Copy-Item -Recurse -Force ../../third_party/mediapipe/* $workspace/
Copy-Item -Recurse -Force ./Native/* $workspace/

Set-Location $workspace

zx ./monkey-patch.mjs

& ../bazel.exe build -c opt `
    --linkopt -s --strip always `
    --define MEDIAPIPE_DISABLE_GPU=1 `
    --define "absl=0" `
    --sandbox_debug --verbose_failures `
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.dll

Copy-Item -Force ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dll ../../liblanguage_detector.dll

Set-Location ../..
Remove-Item -Recurse -Force $workspace

Write-Host "DLL Dependencies:"
if (Get-Command dumpbin -ErrorAction SilentlyContinue) {
    dumpbin /DEPENDENTS liblanguage_detector.dll
} else {
    Write-Warning "You need to install 'Developer Command Prompt for VS'."
}

Write-Host "Goodbye world"
