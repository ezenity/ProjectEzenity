#!/bin/bash

# Navigate to the directory where your .NET project file (.csproj) is located
cd ./Ezenity_Backend

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# If you also have tests, you can run them with
# dotnet test

# If you want to publish the project, uncomment the following line
# dotnet publish -c Release
