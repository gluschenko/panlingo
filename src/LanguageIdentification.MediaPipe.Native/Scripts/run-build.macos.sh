#!/bin/bash
set -euo pipefail

echo "Hello world"

npm install -g zx

brew update

brew install \
  git wget cmake pkg-config curl \
  ffmpeg python@3.11 node@20 openjdk@8 \
  bazelisk llvm@16 protobuf@3 opencv

# Ensure python3 points to Homebrew Python
PYTHON_BIN="$(brew --prefix)/opt/python@3.11/bin/python3"
if [ -x "$PYTHON_BIN" ]; then
  echo "Using Python at $PYTHON_BIN"
  ln -sf "$PYTHON_BIN" /usr/local/bin/python3 || true
  ln -sf "$PYTHON_BIN" /usr/local/bin/python || true
else
  echo "Python3 not found at $PYTHON_BIN" >&2
  exit 1
fi

# Configure JAVA_HOME
export JAVA_HOME="$(/usr/libexec/java_home -v1.8)"

# Link LLVM tools
LLVM_PREFIX="$(brew --prefix llvm@16)"
export PATH="$LLVM_PREFIX/bin:$PATH"

# Install Python packages
pip3 install --upgrade setuptools wheel future absl-py "numpy<2" jax['cpu'] opencv-contrib-python protobuf==3.20.1 six==1.14.0 tensorflow tf_slim

echo -n "Bazel: " && bazel version
echo -n "Python: " && python3 --version
echo -n "Node: " && node --version
echo -n "npm: " && npm --version
echo -n "clang: " && clang --version


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
