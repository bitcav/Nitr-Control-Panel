cd NitrControlPanel
dotnet build --configuration Release
dotnet publish -c Release -p:PublishSingleFile=true --runtime win10-x64 -o Publish
