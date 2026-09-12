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

# Virtual environment
python -m venv venv
source venv/bin/activate

python --version
python -m pip install --upgrade pip

echo -n "Bazel: " && bazel version
echo -n ".NET: " && dotnet --version
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

dotnet run --file ./monkey-patch.cs

bazel build -c opt --compilation_mode=opt \
    --copt=-DHAVE_FDOPEN \
    --linkopt=-s --strip=always \
    --define=MEDIAPIPE_DISABLE_GPU=1 \
    --define=absl=0 \
    --sandbox_debug --verbose_failures \
    //mediapipe/tasks/c/text/language_detector:liblanguage_detector.dylib

artifact=./bazel-bin/mediapipe/tasks/c/text/language_detector/liblanguage_detector.dylib
test -s "$artifact"
echo "Verified MediaPipe artifact from Bazel compilation_mode=opt: $artifact"
cp "$artifact" ../../libmediapipe_language_detector.$ARCH.dylib

cd ../..
rm -rf "$workspace"

otool -L libmediapipe_language_detector.$ARCH.dylib

echo "Goodbye world"
