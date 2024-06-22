#!/bin/bash
set -e

echo "Installing runtime packages";

sudo apt -y update | apt -y update
sudo apt -y install protobuf-compiler libprotobuf-dev | apt -y install protobuf-compiler libprotobuf-dev
