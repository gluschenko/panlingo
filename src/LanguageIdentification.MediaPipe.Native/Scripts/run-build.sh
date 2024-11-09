#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/mediapipe/. $workspace/.
cp -a Native/. $workspace

cd "$workspace"

zx ../Native/monkey-patch.mjs

bazel-6.1.1 build -c opt \
    --linkopt -s --strip always \
    --define MEDIAPIPE_DISABLE_GPU=1 \
    --define='absl=0' \
    --sandbox_debug --verbose_failures \
    --config darwin_arm64 \
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.dylib

cp ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dylib ../liblanguage_detector.dylib

bazel-6.1.1 build -c opt \
    --linkopt -s --strip always \
    --define MEDIAPIPE_DISABLE_GPU=1 \
    --define='absl=0' \
    --sandbox_debug --verbose_failures \
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.so

cp ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.so ../liblanguage_detector.so
rm -rf "$workspace"
ldd liblanguage_detector.so

echo "Goodbye world";

