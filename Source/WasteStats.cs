using DubsBadHygiene;
using RimWorld;
using Verse;

namespace DBHBodySizeWaste
{
    // Informational stats only: no changes to the game's production calculations.
    public sealed class WasteStatWorker : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            Pawn pawn = req.Thing as Pawn;
            return pawn != null && pawn.needs != null && pawn.needs.TryGetNeed<Need_Bladder>() != null;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            Pawn pawn = req.Thing as Pawn;
            float factor = PatchInstaller.Active ? WasteHooks.Factor(pawn) : 1f;
            return Preview(stat.defName, factor, ModOption.FlushSize == null ? 1f : ModOption.FlushSize.Val);
        }

        public static float Preview(string defName, float factor, float flushMultiplier)
        {
            switch (defName)
            {
                case "DBHW_ToiletWaste": return 14f * flushMultiplier * factor;
                case "DBHW_AdvancedWaste": return 7f * flushMultiplier * factor;
                case "DBHW_OutdoorWaste": return 10f * factor;
                default: return factor;
            }
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn == null) return "";
            WasteSettings settings = WasteMod.Settings;
            float factor = PatchInstaller.Active ? WasteHooks.Factor(pawn) : 1f;
            float flush = ModOption.FlushSize == null ? 1f : ModOption.FlushSize.Val;
            string text = "DBHW_StatReport".Translate(pawn.BodySize.ToString("0.###"),
                factor.ToString("0.###"), (flush * 100f).ToString("0.#"));
            if (!PatchInstaller.Active || settings == null || !settings.enabled)
                text += "\n\n" + "DBHW_StatDisabled".Translate();
            else
                text += "\n\n" + "DBHW_StatFormula".Translate(settings.multiplier.ToString("0.##"), settings.exponent.ToString("0.##"));
            text += "\n\n" + "DBHW_StatLimits".Translate();
            return text;
        }
    }
}
