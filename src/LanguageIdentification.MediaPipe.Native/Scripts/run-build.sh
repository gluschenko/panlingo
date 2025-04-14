#!/bin/bash
set -e

echo "Hello world";

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/mediapipe/. $workspace/.
cp -a Native/. $workspace

cd "$workspace"

zx ./monkey-patch.mjs

bazel-6.1.1 build -c opt \
    --linkopt -s --strip always \
    --define MEDIAPIPE_DISABLE_GPU=1 \
    --define='absl=0' \
    --sandbox_debug --verbose_failures \
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.so

cp ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.so ../../liblanguage_detector.so
cd ..
cd ..
rm -rf "$workspace"
ldd liblanguage_detector.so

echo "Goodbye world";

