#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/fastText/. $workspace/fasttext
cp -a Native/. $workspace

ls -R .

ls /usr/lib

cd "$workspace"

mkdir build
cd build

# Build for MacOS
rm -rf *
cmake ..
make -j $(nproc) # make

ls -R

otool -L libfasttext.dylib
cp libfasttext.dylib ../../libfasttext.dylib

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
