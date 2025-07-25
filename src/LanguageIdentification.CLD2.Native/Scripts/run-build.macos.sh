﻿#!/bin/bash
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

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/cld2/. $workspace/cld2
cp -a Native/. $workspace

ls -R .

cd "$workspace"

mkdir build
cd build

echo "Build for MacOS on $ARCH";
rm -rf *
cmake -DCMAKE_OSX_ARCHITECTURES=$ARCH ..
make -j $(sysctl -n hw.logicalcpu) 

ls -R

otool -L libcld2.dylib
cp libcld2.dylib ../../../libcld2.$ARCH.dylib

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
