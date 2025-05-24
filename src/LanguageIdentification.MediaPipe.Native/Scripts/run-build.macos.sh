#!/bin/bash
set -euo pipefail

echo "Hello world"

brew update
HOMEBREW_NO_AUTO_UPDATE=1

# Python
PYTHON311_BIN="/opt/homebrew/opt/python@3.11/bin/python3.11"

if [ ! -x "$PYTHON311_BIN" ]; then
    echo "Installing Python 3.11..."
    brew install python@3.11
fi

echo -n "Using Python binary: " && echo "$PYTHON311_BIN"
echo -n "Python version: " && "$PYTHON311_BIN" --version

# Virtual environment
"$PYTHON311_BIN" -m venv venv
source venv/bin/activate

python --version
python -m pip install --upgrade pip

pip install --break-system-packages --upgrade setuptools wheel future absl-py
pip install --break-system-packages --upgrade tensorflow-macos==2.16.2 tf_slim
pip install --break-system-packages --upgrade "numpy<2" jax['cpu'] opencv-contrib-python protobuf==3.20.1 six==1.14.0
pip install --break-system-packages --upgrade opencv-contrib-python protobuf==3.20.1 six==1.14.0

# Other bullshit
brew install protobuf@3
brew install git 
brew install wget 
brew install cmake 
brew install pkg-config 
brew install curl
brew install ffmpeg
brew install bazelisk
brew install llvm@16

npm install -g zx

echo -n "Bazel: " && bazel version
echo -n "Node: " && node --version
echo -n "npm: " && npm --version
echo -n "clang: " && clang --version

# Configure JAVA_HOME
export JAVA_HOME="$(/usr/libexec/java_home -v1.8)"

# Link LLVM tools
LLVM_PREFIX="$(brew --prefix llvm@16)"
export PATH="$LLVM_PREFIX/bin:$PATH"


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
