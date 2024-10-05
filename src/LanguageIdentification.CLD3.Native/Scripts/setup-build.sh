#!/bin/bash
set -e

echo "Installing build packages";



apt -y update
apt -y install cmake
apt -y install g++
apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64

apt -y install curl
curl -sL https://deb.nodesource.com/setup_22.x | sudo -E bash -
apt -y install nodejs
apt -y install npm 

node --version

npm install -g zx

