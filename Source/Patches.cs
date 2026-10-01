using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DubsBadHygiene;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DBHBodySizeWaste
{
    public static class WasteHooks
    {
        [ThreadStatic] private static Pawn flushingPawn;
        [ThreadStatic] private static bibblefuckwit flushingToilet;

        public static float Factor(Pawn pawn)
        {
            WasteSettings settings = WasteMod.Settings;
            if (pawn == null || settings == null || !settings.enabled) return 1f;
            return WasteMath.Factor(pawn.BodySize, settings.multiplier, settings.exponent);
        }

        public static bool Flush(bibblefuckwit toilet, Pawn pawn)
        {
            Pawn previousPawn = flushingPawn;
            bibblefuckwit previousToilet = flushingToilet;
            try
            {
                flushingPawn = pawn;
                flushingToilet = toilet;
                return toilet.TryUseFlush();
            }
            finally
            {
                flushingPawn = previousPawn;
                flushingToilet = previousToilet;
            }
        }

        public static void FlushSizePostfix(bibblefuckwit __instance, ref float __result)
        {
            // Only scale newly produced waste, never stored sewage or unrelated flushes.
            if (ReferenceEquals(__instance, flushingToilet)) __result *= Factor(flushingPawn);
        }

        public static void GroundSewage(GridLayer grid, IntVec3 cell, float amount,
            bool regen, bool setdirty, MapMeshFlagDef flag, Pawn pawn)
        {
            grid.AddAt(cell, amount * Factor(pawn), regen, setdirty, flag);
        }

        public static bool GroundFilth(IntVec3 cell, Map map, ThingDef def, int count,
            FilthSourceFlags sourceFlags, bool shouldPropagate, Pawn pawn)
        {
            WasteSettings settings = WasteMod.Settings;
            if (settings != null && settings.groundFilth &&
                (def == DubDef.FilthFaeces || def == DubDef.FilthUrine))
            {
                float factor = Factor(pawn);
                if (factor != 1f)
                {
                    // Filth is integral. Stochastic rounding avoids giving tiny pawns
                    // a mandatory full unit each time. Avoid RNG changes at factor 1.
                    float scaled = Math.Min(100f, Math.Max(0f, count * factor));
                    count = (int)scaled;
                    if (scaled > count && Rand.Value < scaled - count) count++;
                }
            }
            if (count <= 0) return false;
            return FilthMaker.TryMakeFilth(cell, map, def, count, sourceFlags, shouldPropagate);
        }
    }

    public static class PatchPlan
    {
        public static readonly MethodInfo FlushCall = AccessTools.Method(typeof(bibblefuckwit), "TryUseFlush", Type.EmptyTypes);
        public static readonly MethodInfo FilthCall = AccessTools.Method(typeof(FilthMaker), "TryMakeFilth",
            new Type[] { typeof(IntVec3), typeof(Map), typeof(ThingDef), typeof(int), typeof(FilthSourceFlags), typeof(bool) });
        public static readonly MethodInfo GridCall = AccessTools.Method(typeof(GridLayer), "AddAt",
            new Type[] { typeof(IntVec3), typeof(float), typeof(bool), typeof(bool), typeof(MapMeshFlagDef) });

        public static MethodInfo FindCallback(string name, MethodInfo requiredCall)
        {
            Type type = typeof(Need_Bladder).Assembly.GetType("DubsBadHygiene." + name, true);
            // Match by actual operation, not compiler-generated method numbering.
            MethodInfo[] matches = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsStatic && m.ReturnType == typeof(void) && m.GetParameters().Length == 0 &&
                    PatchProcessor.GetOriginalInstructions(m).Any(i => i.Calls(requiredCall))).ToArray();
            if (matches.Length != 1) throw new InvalidOperationException(name + ": expected one production callback, found " + matches.Length);
            return matches[0];
        }

        public static MethodInfo[] Targets()
        {
            return new MethodInfo[] {
                FindCallback("JobDriver_UseToilet", FlushCall),
                FindCallback("JobDriver_poopOutside", GridCall),
                AccessTools.Method(typeof(Need_Bladder), "crapPants", Type.EmptyTypes)
            };
        }

        public static List<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            FieldInfo pawnField = AccessTools.Field(typeof(JobDriver).IsAssignableFrom(original.DeclaringType)
                ? typeof(JobDriver) : typeof(Need), "pawn");
            List<CodeInstruction> result = new List<CodeInstruction>();
            int flushes = 0, filths = 0, grids = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                string hook = null;
                if (instruction.Calls(FlushCall)) { hook = "Flush"; flushes++; }
                else if (instruction.Calls(FilthCall)) { hook = "GroundFilth"; filths++; }
                else if (instruction.Calls(GridCall)) { hook = "GroundSewage"; grids++; }
                if (hook == null) { result.Add(instruction); continue; }
                CodeInstruction receiver = new CodeInstruction(OpCodes.Ldarg_0);
                // Preserve branches and exception boundaries on the first replacement instruction.
                receiver.labels.AddRange(instruction.labels);
                receiver.blocks.AddRange(instruction.blocks);
                result.Add(receiver);
                result.Add(new CodeInstruction(OpCodes.Ldfld, pawnField));
                result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WasteHooks), hook)));
            }
            string typeName = original.DeclaringType.Name;
            int expectedFlush = typeName == "JobDriver_UseToilet" ? 1 : 0;
            int expectedGrid = typeName == "JobDriver_poopOutside" ? 1 : 0;
            int expectedFilth = typeName == "JobDriver_UseToilet" ? 1 : 2;
            if (flushes != expectedFlush || grids != expectedGrid || filths != expectedFilth)
                throw new InvalidOperationException("Unexpected DBH waste method layout: " + original +
                    " (flush/filth/grid=" + flushes + "/" + filths + "/" + grids + ")");
            return result;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            return Rewrite(instructions, __originalMethod);
        }
    }

    [StaticConstructorOnStartup]
    public static class PatchInstaller
    {
        public static bool Active;
        static PatchInstaller()
        {
            Harmony harmony = new Harmony("local.dbh.bodysizewaste");
            List<MethodBase> patched = new List<MethodBase>();
            try
            {
                MethodInfo[] targets = PatchPlan.Targets();
                // Validate all producers first; changed DBH versions fail without partial patches.
                foreach (MethodInfo target in targets)
                    PatchPlan.Rewrite(PatchProcessor.GetOriginalInstructions(target), target);
                MethodInfo getter = AccessTools.PropertyGetter(typeof(bibblefuckwit), "flushSize");
                if (getter == null) throw new MissingMethodException("DBH flushSize getter");
                patched.Add(getter);
                harmony.Patch(getter, postfix: new HarmonyMethod(typeof(WasteHooks), "FlushSizePostfix"));
                foreach (MethodInfo target in targets)
                {
                    patched.Add(target);
                    harmony.Patch(target, transpiler: new HarmonyMethod(typeof(PatchPlan), "Transpiler"));
                }
                Active = true;
                Log.Message("[DBH Body Size Waste] Active: toilet sewage, outdoor sewage, and bodily filth. DBH " +
                    typeof(Need_Bladder).Assembly.GetName().Version);
            }
            catch (Exception error)
            {
                foreach (MethodBase method in patched)
                    harmony.Unpatch(method, HarmonyPatchType.All, harmony.Id);
                Log.Error("[DBH Body Size Waste] Disabled: incompatible patch targets. Original behavior retained. " + error);
            }
        }
    }
}
