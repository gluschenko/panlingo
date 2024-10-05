#!/bin/bash
set -e

echo "Installing build packages";

apt-get update && apt-get install -y ca-certificates curl gnupg && \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg && \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list

apt -y update
apt -y install cmake
apt -y install g++
apt -y install gcc-mingw-w64-x86-64 g++-mingw-w64-x86-64
apt -y install nodejs
apt -y install npm 

npm install -g zx

