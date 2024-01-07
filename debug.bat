@echo off
dotnet build src/Limbo.Umbraco.Fetch --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:\nuget\Umbraco9