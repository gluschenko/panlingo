#!/bin/bash
set -e

echo "Hello world";

workspace="obj/native_build_temp"

mkdir "$workspace" -p
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
cp libfasttext.so ../../../libfasttext.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
