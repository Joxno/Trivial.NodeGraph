if(Test-Path -Path .\Trivial.Domain\nupkg)
{
    Get-ChildItem ./Trivial.Domain/nupkg/* | Remove-Item
}

if(Test-Path -Path .\Trivial.Graph\nupkg)
{
    Get-ChildItem ./Trivial.Graph/nupkg/* | Remove-Item
}

#dotnet build .\Trivial.Domain -p:PackageVersion="1.0.0-D$([DateTime]::UtcNow.ToString("yyMMddHHmmss"))"
dotnet build .\Trivial.Graph -p:PackageVersion="1.0.0-D$([DateTime]::UtcNow.ToString("yyMMddHHmmss"))"