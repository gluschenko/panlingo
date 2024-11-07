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

workspace="build_temp"

mkdir "$workspace" -p
cp -a Native/lingua-ffi/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

echo "Build for MacOS on $ARCH";
rm -rf *
cmake -DCMAKE_OSX_ARCHITECTURES=$ARCH ..
make -j $(sysctl -n hw.logicalcpu) 

ls -R

cd ./target/release
otool -L liblingua.dylib
cp liblingua.dylib ../../liblingua.$ARCH.dylib

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
