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

# Build for Linux
cmake ..
make -j $(nproc) # make

./language_identifier_main           # run tests
./language_identifier_features_test  # run tests

ls -R

ldd libcld3.so
cp libcld3.so ../../libcld3.so

rm -rf *

# Build for Windows
cmake .. -DCMAKE_TOOLCHAIN_FILE=./toolchain-mingw.cmake
make -j $(nproc) # make

ls -R

cp libcld3.dll ../../libcld3.dll

rm -rf *

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

