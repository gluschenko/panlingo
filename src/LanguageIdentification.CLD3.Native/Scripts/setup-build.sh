#!/bin/bash
set -e

echo "Installing build packages";

sudo apt -y update | apt -y update
sudo apt -y install g++ | apt -y install g++
sudo apt -y install python3 | apt -y install python3
sudo apt -y install python3-pip | apt -y install python3-pip
sudo apt -y install protobuf-compiler | apt -y install protobuf-compiler

