#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/fastText/. $workspace
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

find "$workspace/build" -name "libfasttext.so.0" -exec cp {} libfasttext.so.0 \;
mv libfasttext.so.0 libfasttext.so
rm -rf "$workspace"
ldd libfasttext.so

echo "Goodbye world";
