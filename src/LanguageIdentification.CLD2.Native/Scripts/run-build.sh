#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/cld2/. $workspace/cld2
cp -a Native/. $workspace

ls -R .

cd "$workspace"

mkdir build
cd build
cmake ..
make -j $(nproc) # make
cd ..

echo $(pwd)
ls -R build
cd ..

find "$workspace/build" -name "libcld2.so" -exec cp {} libcld2.so \;
rm -rf "$workspace"
ldd libcld2.so

echo "Goodbye world";
