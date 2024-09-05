#!/bin/bash
set -e

echo "Installing build packages";

sudo apt -y update | apt -y update
sudo apt -y install cmake | apt -y install cmake
sudo apt -y install g++ | apt -y install g++
sudo apt -y install git | apt -y install git
# sudo apt -y install protobuf-compiler libprotobuf-dev | apt -y install protobuf-compiler libprotobuf-dev

git clone --recurse-submodules --branch v28.x https://github.com/protocolbuffers/protobuf.git
cd protobuf
mkdir build
cd build
cmake -Dprotobuf_BUILD_SHARED_LIBS=ON -Dprotobuf_BUILD_TESTS=OFF ..
make
make install  

