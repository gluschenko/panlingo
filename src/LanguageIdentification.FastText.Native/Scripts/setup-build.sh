#!/bin/bash
set -e

echo "Installing build packages";

sudo apt -y update | apt -y update
sudo apt -y install cmake | apt -y install cmake
sudo apt -y install g++ | apt -y install g++
sudo apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64 | apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64

apt -y install wine wine64 | sudo apt -y install wine wine64

wget https://github.com/Kitware/CMake/releases/download/v3.30.3/cmake-3.30.3-windows-x86_64.msi
wine msiexec /i cmake-3.30.3-windows-x86_64.msi /quiet
wine cmake --version

