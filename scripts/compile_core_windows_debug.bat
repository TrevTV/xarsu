@echo off
cd ..
dotnet restore
dotnet publish -r win-x64 -p:PublishAotUsingRuntimePack=true -p:Configuration=Debug

pause