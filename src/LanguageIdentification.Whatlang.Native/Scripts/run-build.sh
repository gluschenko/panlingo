#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/whatlang-ffi/. $workspace/.
cp -a Native/whatlang-ffi/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

ls -R .

cd ./target/release
ldd libwhatlang.so
cp libwhatlang.so ../../../libwhatlang.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

