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
cp -a Native/whatlang-ffi/. $workspace/.

ls -R .

cd "$workspace"

cargo build --release

ls -R .

cd ./target/release
ldd libwhatlang.so
cp libwhatlang.so ../../../../libwhatlang.$ARCH.so

# Clean up
rm -rf "$workspace"
echo "Goodbye world";

