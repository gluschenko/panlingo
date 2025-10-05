#!/bin/bash
set -e

echo "Hello world";

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/cld2/. $workspace/cld2
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

ldd libcld2.so
cp libcld2.so ../../../libcld2.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

