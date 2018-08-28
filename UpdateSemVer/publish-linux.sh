#!/bin/bash
dotnet publish  update_semver.csproj  -r linux-x64  -p:ShowLinkerSizeComparison=true