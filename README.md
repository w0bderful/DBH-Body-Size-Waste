# DBH - Body Size Waste

A **body-size waste patch for Dubs Bad Hygiene** on RimWorld 1.6 / 1.7.

Default formula: **original amount × actual Pawn.BodySize × configured multiplier**.
A body size of 0.5 produces half the waste, and a body size of 2 produces twice as much.

## Features

- Overall waste multiplier: 0–10×
- Body size influence: 0–2 (0 ignores size, 1 is linear, 2 is squared)
- Optional scaling of ground faeces and urine filth
- Korean and English settings window with a multiplier preview
- Pawn info card (i) shows the waste factor and expected waste per use for regular toilets, advanced toilets and outdoor defecation
- Applies to toilets, latrines and outdoor defecation
- Standalone Harmony patch that does not modify the original mod's files

## Installation

Download the repository and put the top-level folder containing `About`, `1.6`, `1.7`, `Defs`, `Languages`
and `LoadFolders.xml` in `RimWorld/Mods/DBHBodySizeWaste`. The built DLLs are included, so building is optional.

Load order:

1. [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
2. [Dubs Bad Hygiene](https://steamcommunity.com/sharedfiles/filedetails/?id=836308268)
3. DBH - Body Size Waste

Adjust the options under `Options → Mod settings → DBH - Body Size Waste`.
Changes apply from the next use.
Select a pawn and open the info card (i) to see its waste values.
Click an entry to see the body size, the original Flush size, the formula and the conditions that apply.
Reopen the info card after changing settings to refresh it. Pawns without a bladder need show nothing.

## Scope and limits

- Uses the actual `Pawn.BodySize`. Settings that only change graphic size are not reflected.
- Water use, toilet frequency, fixture capacity and treatment speed are unchanged. Bedpans are excluded.
- Stacks on top of DBH's own Flush size setting.
- Latrines fill up to their original capacity. If a single use exceeds the remaining capacity, the excess may be lost as in the original.
- Ground filth is rounded stochastically (0.5 units appear about every other time), up to 100 units per attempt.
- The overall factor is capped at 10000× to guard against abnormal body sizes.
- Other mods with their own toilet jobs or flushSize overrides may need extra compatibility work.

Current target: **RimWorld 1.7 / Dubs Bad Hygiene 3.2.3250**. The 1.6 DLL (built for DBH 3.1.2800) is kept in `1.6/Assemblies`.
Compilation, math tests and Harmony patch emission against the real DLLs pass.
**In-game play and multiplayer have not been tested yet.**

## Building

Requires the .NET 10 SDK and local RimWorld and dependency DLLs. `./Tests/test.ps1` builds the mod, then runs IL and Harmony checks against the real DBH DLL.

```powershell
./Source/build.ps1 -GameDir 'D:\SteamLibrary\steamapps\common\RimWorld' -WorkshopDir 'D:\SteamLibrary\steamapps\workshop\content\294100'
```

Output: `1.7/Assemblies/DBHBodySizeWaste.dll`.
The game, Harmony and Dubs Bad Hygiene DLLs are not included in this repository.

This is not an official Dubwise release.
