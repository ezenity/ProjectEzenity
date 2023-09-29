#!/bin/bash

# Navigate to Ezenity.API
cd ./Ezenity.API

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Navigate to Ezenity.Test Project
cd ../Ezenity.Tests

# Run Ezenity.Tests cases
dotnet test

# Public project
# dotnet publish -c Release
