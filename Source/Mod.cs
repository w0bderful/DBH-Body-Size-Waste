using System;
using UnityEngine;
using Verse;

namespace DBHBodySizeWaste
{
    public sealed class WasteSettings : ModSettings
    {
        public bool enabled = true;
        public float multiplier = 1f;
        public float exponent = 1f;
        public bool groundFilth = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enabled, "enabled", true);
            Scribe_Values.Look(ref multiplier, "multiplier", 1f);
            Scribe_Values.Look(ref exponent, "exponent", 1f);
            Scribe_Values.Look(ref groundFilth, "groundFilth", true);
            multiplier = WasteMath.Valid(multiplier, 1f, 0f, 10f);
            exponent = WasteMath.Valid(exponent, 1f, 0f, 2f);
        }
    }

    public sealed class WasteMod : Mod
    {
        public static WasteSettings Settings;
        public WasteMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<WasteSettings>();
        }

        public override string SettingsCategory() { return "DBH - Body Size Waste"; }

        public override void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard ui = new Listing_Standard();
            ui.Begin(rect);
            ui.Label("DBHW_Intro".Translate());
            if (!PatchInstaller.Active) ui.Label("DBHW_Inactive".Translate());
            ui.Gap();
            ui.CheckboxLabeled("DBHW_Enable".Translate(), ref Settings.enabled);
            ui.Label("DBHW_Multiplier".Translate(Settings.multiplier.ToString("0.00")));
            Settings.multiplier = (float)Math.Round(ui.Slider(Settings.multiplier, 0f, 10f), 2);
            ui.Label("DBHW_Influence".Translate(Settings.exponent.ToString("0.00")));
            Settings.exponent = (float)Math.Round(ui.Slider(Settings.exponent, 0f, 2f), 2);
            ui.Label("DBHW_Formula".Translate());
            ui.CheckboxLabeled("DBHW_Ground".Translate(), ref Settings.groundFilth);
            ui.Gap();
            ui.Label("DBHW_Examples".Translate());
            foreach (float size in new float[] { 0.5f, 1f, 2f, 4f })
            {
                float factor = Settings.enabled ? WasteMath.Factor(size, Settings.multiplier, Settings.exponent) : 1f;
                ui.Label("DBHW_Example".Translate(size.ToString("0.0"), factor.ToString("0.00")));
            }
            ui.Gap();
            ui.Label("DBHW_Notes".Translate());
            if (ui.ButtonText("DBHW_Reset".Translate()))
            {
                Settings.enabled = true;
                Settings.multiplier = 1f;
                Settings.exponent = 1f;
                Settings.groundFilth = true;
            }
            ui.End();
        }
    }
}
