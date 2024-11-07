#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a Native/lingua-ffi/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

ls -R .

cd ..

cd $workspace/target/release
ldd liblingua.so
cp liblingua.so ../../liblingua.so

echo "Goodbye world";

