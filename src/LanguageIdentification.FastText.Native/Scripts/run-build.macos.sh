#!/bin/bash
set -e

echo "Hello world";

brew install llvm
find /opt/homebrew/opt/llvm/lib -type f -name '*.a'
find /opt/homebrew/opt/llvm/lib -type f -name '*.dylib'

arch -x86_64 /usr/local/bin/brew install llvm
find /usr/local/opt/llvm/lib -type f -name '*.a'
find /usr/local/opt/llvm/lib -type f -name '*.dylib'

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/fastText/. $workspace/fasttext
cp -a Native/. $workspace

ls -R .

cd "$workspace"

mkdir build
cd build

echo "Build for MacOS on x86";
rm -rf *
arch -x86_64 /bin/bash -c "cmake -DCMAKE_OSX_ARCHITECTURES=x86_64 -DCMAKE_OSX_DEPLOYMENT_TARGET=10.15 .."
arch -x86_64 /bin/bash -c "make -j $(sysctl -n hw.logicalcpu)"

ls -R

otool -L libfasttext.dylib
cp libfasttext.dylib ../../libfasttext.x86_64.dylib

echo "Build for MacOS on M1";
rm -rf *
cmake -DCMAKE_OSX_ARCHITECTURES=arm64 ..
make -j $(sysctl -n hw.logicalcpu) 

ls -R

otool -L libfasttext.dylib
cp libfasttext.dylib ../../libfasttext.arm64.dylib

echo "Make universal binary";
lipo -create ../../libfasttext.x86_64.dylib ../../libfasttext.arm64.dylib -output ../../libfasttext.dylib

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
