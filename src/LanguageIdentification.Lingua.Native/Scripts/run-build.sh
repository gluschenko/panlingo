#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a Native/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

ls -R .

cd ..

find "$workspace/target/release" -name "liblingua.so" -exec cp {} liblingua.so \;
rm -rf "$workspace"
ldd liblingua.so

echo "Goodbye world";

