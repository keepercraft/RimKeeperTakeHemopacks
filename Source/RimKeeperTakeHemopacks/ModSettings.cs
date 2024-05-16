using Keepercraft.RimKeeperTakeHemopacks.Extensions;
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

            float x = listingStandard.GetPrivateField<float>("curX");
            float y = listingStandard.GetPrivateField<float>("curY");
            Texture2D HemogenpackIcon = ContentFinder<Texture2D>.Get("Things/Item/Resource/HemogenPack/HemogenPack_c");
            GUI.DrawTexture(new Rect(x, y, 30, 30), HemogenpackIcon); //listingStandard.ButtonImage(HemogenpackIcon, 30, 30);
            listingStandard.Gap(60);
            
            listingStandard.CheckboxLabeled("Enable shareing", ref KeeperModSettings.HemogenInventoryShare, "Vampire colonists will be share hemopacks");
            listingStandard.Gap();

            listingStandard.Label("How many take to inventory:");
            listingStandard.IntEntry(ref KeeperModSettings.HemogenInventoryLimit, ref HemogenInventoryLimitText, 1);
            listingStandard.Gap();
            listingStandard.Label("Inventory threshold for start looking:");
            listingStandard.IntEntry(ref KeeperModSettings.HemogenInventoryThreshold, ref HemogenInventoryThresholdText, 1);

            listingStandard.Gap();
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
