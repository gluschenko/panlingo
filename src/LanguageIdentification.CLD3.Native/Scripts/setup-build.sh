#!/bin/bash
set -e

echo "Installing build packages";

sudo apt -y update | apt -y update
sudo apt -y install cmake | apt -y install cmake
sudo apt -y install g++ | apt -y install g++
sudo apt -y install git | apt -y install git
# sudo apt -y install protobuf-compiler libprotobuf-dev | apt -y install protobuf-compiler libprotobuf-dev

git clone --recurse-submodules --depth 1 --branch v28.0 https://github.com/protocolbuffers/protobuf.git
cd protobuf
mkdir build
cd build
cmake -DCMAKE_POSITION_INDEPENDENT_CODE=ON -Dprotobuf_BUILD_TESTS=OFF ..
make -j $(nproc)

###

rm -rf union_temp
mkdir union_temp

find . -name "*.a" | while read lib; do
  echo "Extracting objects from $lib"
  ar -x "$lib" --output union_temp
done

echo "Creating combined archive libprotobuf.a"
ar -qc libprotobuf.a union_temp/*.o

ranlib libprotobuf.a

rm -rf union_temp

echo "Combined archive libprotobuf.a created successfully."

###

make install  

cd ../..
