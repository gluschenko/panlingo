#!/bin/bash
set -e

echo "Installing build packages";

sudo apt -y update | apt -y update
sudo apt -y install cmake | apt -y install cmake
sudo apt -y install g++ | apt -y install g++
sudo apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64 | apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64
