#!/bin/bash
set -e

echo "Installing build packages";

apt -y update
apt -y install curl dirmngr apt-transport-https lsb-release ca-certificates
curl -sL https://deb.nodesource.com/setup_20.x | bash -

apt -y update
apt -y install cmake
apt -y install g++
apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64

apt -y install nodejs
apt -y install npm 

node --version

npm install -g zx

