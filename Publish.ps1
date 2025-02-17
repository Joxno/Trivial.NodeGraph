param($NugetApiKey)

$PackageName = Get-ChildItem -Path .\Trivial.Graph\nupkg\*.nupkg | select -first 1 -ExpandProperty Name
dotnet nuget push "./Trivial.Graph/nupkg/$PackageName" -k $NugetApiKey -s https://api.nuget.org/v3/index.json

$PackageName = Get-ChildItem -Path .\Trivial.Domain\nupkg\*.nupkg | select -first 1 -ExpandProperty Name
dotnet nuget push "./Trivial.Domain/nupkg/$PackageName" -k $NugetApiKey -s https://api.nuget.org/v3/index.json