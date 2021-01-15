#!/bin/bash
cd -- "$(dirname "$BASH_SOURCE")"
echo "Fixed the path"
cd ./ZoomAutoJoin
echo "Let's go, I'm starting with dotnet build"
dotnet build -r osx-x64
echo "Built successfully! Navigating to application path"
cd ./bin/Debug/net5.0/osx-x64/
echo "Reached the location, starting the Application"
./ZoomAutoJoin
