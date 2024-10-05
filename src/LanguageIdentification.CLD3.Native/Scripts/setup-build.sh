#!/bin/bash
set -e

echo "Installing build packages";

apt -y update
apt -y install curl dirmngr apt-transport-https lsb-release ca-certificates
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.5/install.sh | bash
source ~/.bashrc
nvm install 22

apt -y update
apt -y install cmake
apt -y install g++
apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64

node --version
npm --version

npm install -g zx

