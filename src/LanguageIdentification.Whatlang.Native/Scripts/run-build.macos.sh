﻿#!/bin/bash
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

brew install llvm

workspace="obj/native_build_temp"

mkdir -p "$workspace"
cp -a Native/whatlang-ffi/. $workspace/.

ls -R .

cd "$workspace"

echo "Build for MacOS on $ARCH";
cargo build --release

ls -R

cd ./target/release
otool -L libwhatlang.dylib
cp libwhatlang.dylib ../../../../libwhatlang.$ARCH.dylib

# Clean up
rm -rf "$workspace"
echo "Goodbye world";
