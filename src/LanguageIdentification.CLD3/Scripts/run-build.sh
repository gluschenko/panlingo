#!/bin/bash
set -e

echo "Hello world";

workspace="build_temp"

mkdir "$workspace" -p
cp -a ../../third_party/. $workspace
ls -R .

rm "$workspace/setup.py"
cp Native/setup.py "$workspace/setup.py"
cp Native/bind.cc "$workspace/src/bind.cc"
cp Native/bind.h "$workspace/src/bind.h"

cd "$workspace"
export PYTHONPATH=$(pwd)/site-packages
echo "$PYTHONPATH"
python3 -m pip install -r requirements.txt --break-system-packages --target="$PYTHONPATH"
python3 setup.py build_ext

ls -R build

cd ..
find "$workspace/build" -name "libcld3.so" -exec cp {} libcld3.so \;
rm -rf "$workspace"
ldd libcld3.so

echo "Goodbye world";
