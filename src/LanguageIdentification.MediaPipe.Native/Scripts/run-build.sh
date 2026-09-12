#!/bin/bash
set -e

if [ -z "$1" ]; then
    echo "Error: No architecture specified."
    echo "Usage: $0 <arch>"
    exit 1
fi

ARCH=$1

if [[ "$ARCH" != "x86_64" && "$ARCH" != "arm64" ]]; then
    echo "Error: Invalid architecture specified. Use 'x86_64' or 'arm64'."
    exit 1
fi

echo "Hello world $ARCH";

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/mediapipe/. $workspace/.
cp -a Native/. $workspace

cd "$workspace"

dotnet run --file ./monkey-patch.cs

bazel build -c opt --compilation_mode=opt \
    --linkopt -s --strip always \
    --define MEDIAPIPE_DISABLE_GPU=1 \
    --define='absl=0' \
    --sandbox_debug --verbose_failures \
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.so

artifact=./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.so
test -s "$artifact"
echo "Verified MediaPipe artifact from Bazel compilation_mode=opt: $artifact"
cp "$artifact" ../../libmediapipe_language_detector.$ARCH.so
cd ..
cd ..
rm -rf "$workspace"
ldd libmediapipe_language_detector.$ARCH.so

echo "Goodbye world";
