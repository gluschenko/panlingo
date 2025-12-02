#!/bin/bash
set -e

if [ -z "$1" ]; then
    echo "Error: No architecture specified."
    echo "Usage: $0 <arch>"
    exit 1
fi

ARCH=$1

if [[ "$ARCH" != "x86_64" && "$ARCH" != "arm64" ]]; then
    echo "Error: Invalid architecture specified. Use 'x86_64' or 'arm64'."
    exit 1
fi

echo "Hello world $ARCH";

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
cp libcld3.so ../../../libcld3.$ARCH.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

