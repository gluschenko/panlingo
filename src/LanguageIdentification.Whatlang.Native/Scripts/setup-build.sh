#!/bin/bash
set -e

echo "Installing build packages";

sudo apt -y update | apt -y update
sudo apt -y install cargo | apt -y install cargo
