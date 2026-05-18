#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

echo "Updating package index and installing dependencies..."
sudo apt update
sudo apt install -y ca-certificates curl

# Create directory for keyrings if it doesn't exist
sudo install -m 0755 -d /etc/apt/keyrings

# Add Docker's official GPG key
echo "Downloading Docker GPG key..."
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

# Add the repository to Apt sources
echo "Adding Docker repository to sources..."
sudo tee /etc/apt/sources.list.d/docker.sources <<EOF
Types: deb
URIs: https://download.docker.com/linux/ubuntu
Suites: $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}")
Components: stable
Architectures: $(dpkg --print-architecture)
Signed-By: /etc/apt/keyrings/docker.asc
EOF

# Update the package index again to include Docker's repo
sudo apt update

echo "Docker repository setup complete!"