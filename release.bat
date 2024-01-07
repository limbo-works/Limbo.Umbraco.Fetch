@echo off
dotnet build src/Limbo.Umbraco.Fetch --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget