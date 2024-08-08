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
    --sandbox_debug --verbose_failures \
	//mediapipe/tasks/c/text/language_detector:liblanguage_detector.so

cd ..

find "$workspace/bazel-bin/mediapipe/tasks/c/text/language_detector" -name "liblanguage_detector.so" -exec cp {} liblanguage_detector.so \;
rm -rf "$workspace"
ldd liblanguage_detector.so

# Download the TFLite model
wget https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite

echo "Goodbye world";

