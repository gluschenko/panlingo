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

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/fastText/. $workspace/fasttext
cp -a Native/. $workspace

ls -R .

cd "$workspace"

mkdir build
cd build

# Build for Linux
rm -rf *
cmake ..
make -j $(nproc) # make

ls -R

ldd libfasttext.so
cp libfasttext.so ../../../libfasttext.$ARCH.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
