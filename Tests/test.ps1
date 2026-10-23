param(
    [string]$GameDir = 'D:\SteamLibrary\steamapps\common\RimWorld',
    [string]$WorkshopDir = 'D:\SteamLibrary\steamapps\workshop\content\294100'
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
& "$root\Source\build.ps1" -GameDir $GameDir -WorkshopDir $WorkshopDir
& dotnet build "$PSScriptRoot\Validate.csproj" -nologo -v q "-p:GameDir=$GameDir" "-p:WorkshopDir=$WorkshopDir"
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
& dotnet "$PSScriptRoot\bin\Validate.dll" $root $GameDir $WorkshopDir
if ($LASTEXITCODE -ne 0) { throw 'Validation failed' }
