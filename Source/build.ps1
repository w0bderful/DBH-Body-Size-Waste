param(
    [string]$GameDir = 'D:\SteamLibrary\steamapps\common\RimWorld',
    [string]$WorkshopDir = 'D:\SteamLibrary\steamapps\workshop\content\294100'
)
$ErrorActionPreference = 'Stop'
$modRoot = Split-Path $PSScriptRoot -Parent
# RimWorld 1.7 runs on .NET 10; the 1.6 DLL in 1.6/Assemblies is kept as built for 1.6.
& dotnet build "$PSScriptRoot\DBHBodySizeWaste.csproj" -c Release -nologo -v q "-p:GameDir=$GameDir" "-p:WorkshopDir=$WorkshopDir"
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
Write-Output "Built $modRoot\1.7\Assemblies\DBHBodySizeWaste.dll"
