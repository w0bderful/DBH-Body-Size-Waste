DBH - Body Size Waste
Version: 1.2.0 (2026-10-24)
Target: RimWorld 1.7 / Dubs Bad Hygiene 3.2.3250 (the 1.6 DLL is kept in 1.6/Assemblies)

How to use
1. Enable Harmony, Dubs Bad Hygiene and DBH - Body Size Waste in that order.
2. Open Options > Mod settings > DBH - Body Size Waste.
3. Adjust the overall multiplier (0-10) and body size influence (0-2).
   Default formula: original waste x actual BodySize x overall multiplier.
   Changing the influence uses BodySize^influence instead of BodySize.
   Influence 0 = ignore size, 1 = linear, 2 = squared.
4. Changes apply from the next use. Waste already stored is not changed.

Default examples
The pawn info card (i) shows the waste factor and the expected waste per use for each toilet type.
It is shown for pawns with a bladder need. Select an entry to see the formula and limits.
Reopen the info card after changing settings to see the latest values.

Body size 0.5: 0.5x the original
Body size 1.0: same as the original
Body size 2.0: 2x the original
Body size 4.0: 4x the original
Setting the overall multiplier to 2 doubles all of the values above.

Scope
- Regular/advanced toilets and latrines: scales the sewage produced at the moment of use.
- Waste collected from latrines: reflected naturally through the accumulated sewage.
- Outdoor defecation: scales the sewage added to the ground.
- Ground faeces/urine: optional in settings. Fractions are rounded stochastically.
  Example: 0.5 units appear about every other time on average. Ground filth is attempted
  up to 100 units at once; actual display and accumulation follow the game's terrain and filth limits.
- Uses the actual Pawn.BodySize, including changes from age, race, genes and other mods.
  Mods that only change graphic size are not reflected.

Unchanged behavior and limits
- Water use, toilet frequency, treatment speed and tank/toilet capacity of the original are unchanged.
- Stacks on top of DBH's own Flush size setting.
- Latrines fill up to their original capacity. If a single use exceeds the remaining
  capacity, the excess may be lost as handled by the original cap.
- Zero waste from futuristic toilets and skipping in Lite Mode are kept.
- Bedpans are excluded: they create one patient item and delete it when cleaned, a separate system.
- Other mods with their own toilet jobs or flushSize overrides may need extra compatibility work.
- The overall factor is capped at 10000x to guard against abnormal body sizes.
- Multiplayer sync has not been tested. Made for single player.

Safe patching
A standalone Harmony patch that does not modify the original workshop files.
Nothing is recalculated when sewage moves or is cleaned, so the cleaner's size does not matter.
If the patched DBH structure changes, an error is logged and this patch removes itself.
You can confirm it is active by this line in Player.log:
[DBH Body Size Waste] Active:

Verification
- Compiles against the installed RimWorld/DBH/Harmony DLLs with 0 warnings.
- Waste math checks for size, multiplier, exponent and invalid input pass.
- Real IL targets and replacement call signatures for toilets, outdoor defecation and accidents pass.
- Real Harmony patch emission: 3 waste callbacks + flushSize getter pass.
- In-game play and settings window rendering have not been run yet.
  After first launch, compare the increase when pawns of different sizes use a toilet.

Development
Source/build.ps1 rebuilds against the locally installed DLLs (.NET 10 SDK required).
Tests/test.ps1 checks the IL and Harmony patch emission against the real DBH DLL.
The original game/mod DLLs are not included in the release.
