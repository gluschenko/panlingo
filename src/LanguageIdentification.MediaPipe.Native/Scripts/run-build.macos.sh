#!/bin/bash
set -euo pipefail

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

export HOMEBREW_NO_AUTO_UPDATE=1
export HOMEBREW_NO_INSTALL_CLEANUP=1
brew update

python --version

# Python
PYTHON311_BIN="/opt/homebrew/opt/python@3.11/bin/python3.11"
if [[ "$ARCH" == "x86_64" ]]; then
    PYTHON311_BIN="/usr/local/opt/python@3.11/bin/python3.11"
fi

if [ ! -x "$PYTHON311_BIN" ]; then
    echo "Installing Python 3.11..."
    brew install python-tk@3.11
    brew install python@3.11
fi

echo -n "Using Python binary: " && echo "$PYTHON311_BIN"
echo -n "Python version: " && "$PYTHON311_BIN" --version

# Virtual environment
"$PYTHON311_BIN" -m venv venv
source venv/bin/activate

"$PYTHON311_BIN" --version
"$PYTHON311_BIN" -m pip install --upgrade pip

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

cp ./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dylib ../../libmediapipe_language_detector.$ARCH.dylib

cd ../..
rm -rf "$workspace"

otool -L libmediapipe_language_detector.$ARCH.dylib

echo "Goodbye world"
