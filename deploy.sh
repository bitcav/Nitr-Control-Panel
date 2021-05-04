mkdir Release
cd NitrControlPanel
dotnet build --configuration Release
dotnet publish -c Release -o test
