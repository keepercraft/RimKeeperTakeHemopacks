using HarmonyLib;
using Keepercraft.RimKeeperTakeHemopacks.Extensions;
using Keepercraft.RimKeeperTakeHemopacks.Helpers;
using RimWorld;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Verse;

namespace Keepercraft.RimKeeperTakeHemopacks
{
    public class KeeperModSettings : ModSettings
    {
        public static bool DebugLog = false;
        public static bool HemogenInventoryShare = true;
        public static int HemogenInventoryLimit = 4;
        public static int HemogenInventoryThreshold = 1;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref DebugLog, nameof(DebugLog));
            Scribe_Values.Look(ref HemogenInventoryLimit, nameof(HemogenInventoryLimit));
            Scribe_Values.Look(ref HemogenInventoryThreshold, nameof(HemogenInventoryThreshold));
            base.ExposeData();
        }

        public static List<ThingDef> hemogenPacks = new List<ThingDef>();

        public static void LoadHemopacks()
        {
            new List<ThingDef>()
            {
                DefDatabase<ThingDef>.GetNamed("HemogenPack", false),
                DefDatabase<ThingDef>.GetNamed("VRE_HemogenPack_Sanguophage", false),
                DefDatabase<ThingDef>.GetNamed("VRE_HemogenPack_Animal", false),
                DefDatabase<ThingDef>.GetNamed("VRE_HemogenPack_Corpse", false),
            }.Where(w => w != null)
            .Do(c => hemogenPacks.Add(c));
            FindMoreHemogenPacks();
        }

        public static void FindMoreHemogenPacks() => DefDatabase<ThingDef>.AllDefs
            .Where(w => !hemogenPacks.Contains(w))
            .Where(w => w.defName.Contains("HemogenPack"))
            .Where(w => w.thingCategories.Any(a => a.defName == "Foods"))
            .Do(c => hemogenPacks.Add(c));
    }

    public class RimKeeperTakeHemopacksMod : Mod
    {
        public Texture2D HemogenpackIcon;
        public string HemogenInventoryLimitText = "0";
        public string HemogenInventoryThresholdText = "0";

        public RimKeeperTakeHemopacksMod(ModContentPack content) : base(content)
        {
            GetSettings<KeeperModSettings>();
            HemogenInventoryLimitText = KeeperModSettings.HemogenInventoryLimit.ToString();
            HemogenInventoryThresholdText = KeeperModSettings.HemogenInventoryThreshold.ToString();
        }

        public override string SettingsCategory() => "RK TakeHemopacks";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            Rect newRect = new Rect(inRect.x, inRect.y, inRect.width / 2, inRect.height);
            listingStandard.Begin(newRect);

            listingStandard.CheckboxLabeled("Debug Log", ref KeeperModSettings.DebugLog, "Log Messages");
            listingStandard.Gap();

            //float x = listingStandard.GetPrivateField<float>("curX");
            //float y = listingStandard.GetPrivateField<float>("curY");
            //Texture2D HemogenpackIcon = ContentFinder<Texture2D>.Get("Things/Item/Resource/HemogenPack/HemogenPack_c");
            //GUI.DrawTexture(new Rect(x, y, 30, 30), HemogenpackIcon); //listingStandard.ButtonImage(HemogenpackIcon, 30, 30);
            //listingStandard.Gap(60);
            
            listingStandard.CheckboxLabeled("Enable shareing", ref KeeperModSettings.HemogenInventoryShare, "Vampire colonists will be share hemopacks");
            listingStandard.Gap();

            listingStandard.Label("How many take to inventory per stack:");
            listingStandard.IntEntry(ref KeeperModSettings.HemogenInventoryLimit, ref HemogenInventoryLimitText, 1);
            listingStandard.Gap();
            listingStandard.Label("Inventory threshold for start looking:");
            listingStandard.IntEntry(ref KeeperModSettings.HemogenInventoryThreshold, ref HemogenInventoryThresholdText, 1);

            listingStandard.Gap();
            listingStandard.End();

            Rect newRectRight = new Rect(inRect.x + (inRect.width / 2) + 20, inRect.y, inRect.width / 2, inRect.height);
            GUI.Label(new Rect(newRectRight.x, newRectRight.y, newRectRight.width, 30), "Detected hemogen packs:");
            float cell_size = 30;
            int i = 0;
            foreach (var item in KeeperModSettings.hemogenPacks)
            {
                i++;
                GUI.DrawTexture(new Rect(newRectRight.x + 10, newRectRight.y + (cell_size * i), cell_size, cell_size), item.uiIcon);
                GUI.Label(new Rect(newRectRight.x + 50, newRectRight.y + (cell_size * i) + 5, newRectRight.width - cell_size, cell_size), item.label);
            }
            
            base.DoSettingsWindowContents(inRect);
        }
    }
}
