#!/bin/bash
set -e

echo "Installing tools if not present..."
dotnet tool install --global coverlet.console 2>/dev/null || true
dotnet tool install --global dotnet-reportgenerator-globaltool 2>/dev/null || true

echo "Restoring and building..."
dotnet restore Ambev.DeveloperEvaluation.sln
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore

echo "Running tests with coverage..."
DOTNET_ROLL_FORWARD=Major dotnet test Ambev.DeveloperEvaluation.sln \
  --no-restore \
  --verbosity normal \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:CoverletOutput=./TestResults/coverage.cobertura.xml \
  /p:Exclude="[*]*.Program,[*]*.Startup,[*]*.Migrations.*"

echo "Generating HTML report..."
reportgenerator \
  -reports:"./tests/**/TestResults/coverage.cobertura.xml" \
  -targetdir:"./TestResults/CoverageReport" \
  -reporttypes:Html

echo ""
echo "Coverage report generated at: TestResults/CoverageReport/index.html"
open TestResults/CoverageReport/index.html 2>/dev/null || true
