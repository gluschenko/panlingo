#!/bin/bash
set -e

echo "Installing build packages";

apt -y update
apt -y install curl dirmngr apt-transport-https lsb-release ca-certificates
apt -y install cmake
apt -y install g++
apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64



