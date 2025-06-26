using HarmonyLib;
using Keepercraft.RimKeeperTakeHemopacks.Helpers;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Keepercraft.RimKeeperTakeHemopacks.Models
{
    public class RimKeeperTakeHemopacksModSettings : ModSettingsInit
    {
        public static bool DebugLog = false;
        public static bool HemogenInventoryShare = true;
        public static int HemogenInventoryLimit = 4;
        public static int HemogenInventoryThreshold = 1;

        public override void ExposeData()
        {
            base.ExposeData();
            DebugHelper.Active = DebugLog;
            Scribe_Values.Look(ref DebugLog, nameof(DebugLog));
            Scribe_Values.Look(ref HemogenInventoryLimit, nameof(HemogenInventoryLimit));
            Scribe_Values.Look(ref HemogenInventoryThreshold, nameof(HemogenInventoryThreshold));
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
}