#!/bin/bash
set -euo pipefail

echo "Hello world"

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/mediapipe/. "$workspace"/
cp -a Native/. "$workspace"/

cd "$workspace"

zx ./monkey-patch.mjs

bazel build -c opt \
    --linkopt=-s --strip=always \
    --define=MEDIAPIPE_DISABLE_GPU=1 \
    --define=absl=0 \
    --sandbox_debug --verbose_failures \
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.dylib

# Копируем результат сборки
cp ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dylib ../../liblanguage_detector.dylib

# Убираем временную папку
cd ../..
rm -rf "$workspace"

# Проверка зависимостей через otool на macOS (аналог ldd в Linux)
otool -L liblanguage_detector.dylib

echo "Goodbye world"
