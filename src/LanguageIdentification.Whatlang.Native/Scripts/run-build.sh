#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/whatlang-ffi/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

ls -R .

cd ..

find "$workspace/target/release" -name "libwhatlang.so" -exec cp {} libwhatlang.so \;
rm -rf "$workspace"
ldd libwhatlang.so

echo "Goodbye world";

