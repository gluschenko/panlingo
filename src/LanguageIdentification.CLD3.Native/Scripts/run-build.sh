#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/cld3/. $workspace/.
cp -a Native/. $workspace

ls -R .

cd "$workspace"

zx ../Native/monkey-patch.mjs

mkdir build
cd build
cmake ..
make -j $(nproc) # make

./language_identifier_main           # run tests
./language_identifier_features_test  # run tests

cd ..

echo $(pwd)
ls -R build
cd ..

find "$workspace/build" -name "libcld3.so" -exec cp {} libcld3.so \;
rm -rf "$workspace"
ldd libcld3.so

echo "Goodbye world";

