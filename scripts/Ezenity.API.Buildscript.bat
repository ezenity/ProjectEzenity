@echo off

REM Navigate up to the root directory
cd ..

REM Navigate to Ezenity.API
cd ./Ezenity.API

REM Restore dependencies
dotnet restore

REM Build the project
dotnet build

REM Navigate to Ezenity.Test Project
cd ../Ezenity.Tests

REM Run Ezenity.Tests cases
dotnet test

REM Publish project
REM dotnet publish -c Release
