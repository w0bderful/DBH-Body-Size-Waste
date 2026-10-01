param(
    [string]$GameDir = 'D:\SteamLibrary\steamapps\common\RimWorld',
    [string]$WorkshopDir = 'D:\SteamLibrary\steamapps\workshop\content\294100'
)
$ErrorActionPreference = 'Stop'
$modRoot = Split-Path $PSScriptRoot -Parent
$managed = Join-Path $GameDir 'RimWorldWin64_Data\Managed'
$output = Join-Path $modRoot 'Assemblies'
New-Item -ItemType Directory -Force -Path $output | Out-Null
$refs = @(
    "$managed\mscorlib.dll",
    "$managed\System.dll",
    "$managed\System.Core.dll",
    "$managed\Assembly-CSharp.dll",
    "$managed\UnityEngine.CoreModule.dll",
    "$managed\UnityEngine.IMGUIModule.dll",
    "$managed\UnityEngine.TextRenderingModule.dll",
    "$managed\netstandard.dll",
    "$WorkshopDir\836308268\1.6\Assemblies\BadHygiene.dll",
    "$WorkshopDir\2009463077\Current\Assemblies\0Harmony.dll"
)
$argsList = @('/nologo', '/noconfig', '/nostdlib+', '/target:library', '/optimize+', '/langversion:5', "/out:$output\DBHBodySizeWaste.dll")
$argsList += $refs | ForEach-Object { '/reference:' + $_ }
$argsList += Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.cs' | Select-Object -ExpandProperty FullName
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" @argsList
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
Write-Output "Built $output\DBHBodySizeWaste.dll"
