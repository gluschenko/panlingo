#!/bin/bash
set -e

echo "Hello world";

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a ../../third_party/cld3/. $workspace/.
cp -a Native/. $workspace

ls -R .

cd "$workspace"

curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.5/install.sh | bash
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
nvm install 22
nvm use 22
npm install -g zx

zx ./monkey-patch.mjs

mkdir build
cd build

# Build for Linux
rm -rf *
cmake ..
make -j $(nproc) # make

./language_identifier_main           # run tests
./language_identifier_features_test  # run tests

ls -R

ldd libcld3.so
cp libcld3.so ../../../libcld3.so

# Build for Windows
rm -rf *
cmake .. -DCMAKE_TOOLCHAIN_FILE=./toolchain-mingw.cmake
make -j $(nproc) # make

ls -R

cp libcld3.dll ../../../libcld3.dll

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

