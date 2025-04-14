#!/bin/bash
set -e

echo "Hello world";

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a Native/lingua-ffi/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

ls -R .

cd ./target/release
ldd liblingua.so
cp liblingua.so ../../../../liblingua.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

