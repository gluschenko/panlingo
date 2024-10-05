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

brew install llvm
npm install -g zx

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/cld3/. $workspace/.
cp -a Native/. $workspace

ls -R .

cd "$workspace"

zx ../Native/monkey-patch.mjs

mkdir build
cd build

echo "Build for MacOS on $ARCH";
rm -rf *
cmake -DCMAKE_OSX_ARCHITECTURES=$ARCH ..
make -j $(sysctl -n hw.logicalcpu) 

ls -R

otool -L libcld3.dylib
cp libcld3.dylib ../../libcld3.$ARCH.dylib

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
