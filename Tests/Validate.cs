using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using DBHBodySizeWaste;
using DubsBadHygiene;
using HarmonyLib;

// Checks the shipped DBH IL and Harmony emission outside the game. No rendering or ticks run.
class Validate
{
    static int Main(string[] args)
    {
        string root = args[0], game = args[1], workshop = args[2];
        AssemblyLoadContext.Default.Resolving += delegate(AssemblyLoadContext context, AssemblyName name)
        {
            foreach (string path in new[] { Path.Combine(root, "1.7", "Assemblies"), Path.Combine(workshop, "836308268", "1.7", "Assemblies"),
                Path.Combine(workshop, "2009463077", "1.7", "Assemblies"), game })
            {
                string file = Path.Combine(path, name.Name + ".dll");
                if (File.Exists(file)) return context.LoadFromAssemblyPath(file);
            }
            return null;
        };
        try { Run(); return 0; } catch (Exception e) { Console.WriteLine(e); return 1; }
    }

    static void Expect(bool value, string label) { if (!value) throw new Exception(label); }

    static void Run()
    {
        Expect(WasteMath.Factor(0.5f, 1f, 1f) == 0.5f && WasteMath.Factor(2f, 1f, 1f) == 2f, "Linear body size");
        Expect(WasteMath.Factor(2f, 1f, 2f) == 4f && WasteMath.Factor(3f, 1f, 0f) == 1f, "Exponent");
        Expect(WasteMath.Factor(2f, 2f, 1f) == 4f && WasteMath.Factor(1f, 0f, 1f) == 0f, "Multiplier");
        Expect(WasteMath.Factor(float.NaN, 1f, 1f) == 1f && WasteMath.Factor(-1f, 1f, 1f) == 0f, "Invalid body size");
        Expect(WasteMath.Factor(1f, float.PositiveInfinity, 1f) == 1f && WasteMath.Factor(1f, 50f, 1f) == 10f, "Multiplier clamp");
        Expect(WasteMath.Factor(1e9f, 10f, 2f) == 10000f, "Overall cap");
        Console.WriteLine("PASS: waste math");

        Expect(PatchPlan.FlushCall != null && PatchPlan.FilthCall != null && PatchPlan.GridCall != null, "Replaced call signatures");
        MethodInfo getter = AccessTools.PropertyGetter(typeof(bibblefuckwit), "flushSize");
        Expect(getter != null && getter.ReturnType == typeof(float), "flushSize getter");
        MethodInfo[] targets = PatchPlan.Targets();
        foreach (MethodInfo target in targets)
        {
            // Throws when the producer layout changed (call counts per method).
            PatchPlan.Rewrite(PatchProcessor.GetOriginalInstructions(target), target);
            Console.WriteLine("  " + target.DeclaringType.FullName + "." + target.Name);
        }
        Console.WriteLine("PASS: " + targets.Length + " DBH producers match expected IL layout");

        var harmony = new Harmony("local.dbh.bodysizewaste.validation");
        try
        {
            harmony.Patch(getter, postfix: new HarmonyMethod(typeof(WasteHooks), "FlushSizePostfix"));
            foreach (MethodInfo target in targets)
                harmony.Patch(target, transpiler: new HarmonyMethod(typeof(PatchPlan), "Transpiler"));
            Console.WriteLine("PASS: Harmony emitted " + (targets.Length + 1) + " patches on actual DBH " + typeof(Need_Bladder).Assembly.GetName().Version);
        }
        finally { harmony.UnpatchAll(harmony.Id); }
    }
}
