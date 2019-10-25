#!/bin/bash
set -euo pipefail
dotnet run --project "./tools/FakeItEasy.Build/FakeItEasy.Build.csproj" -- $@

