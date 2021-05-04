cd NitrControlPanel
dotnet build --configuration Release
dotnet publish -c Release -p:PublishSingleFile=true --runtime win-x64 --self-contained false -o Publish
