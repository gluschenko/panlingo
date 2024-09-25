#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

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
cp libfasttext.so ../../libfasttext.so

# Build for Windows
rm -rf *
wine cmake ..
wine cmake --build .

ls -R

cp libfasttext.dll ../../libfasttext.dll

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
