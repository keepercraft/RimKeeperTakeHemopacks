using Keepercraft.RimKeeperTakeHemopacks.Models;
using UnityEngine;
using Verse;

namespace Keepercraft.RimKeeperTakeHemopacks
{
    public class RimKeeperTakeHemopacksMod : Mod
    {
        public Texture2D HemogenpackIcon;
        public string HemogenInventoryLimitText = "0";
        public string HemogenInventoryThresholdText = "0";

        public RimKeeperTakeHemopacksMod(ModContentPack content) : base(content)
        {
            GetSettings<RimKeeperTakeHemopacksModSettings>();
            HemogenInventoryLimitText = RimKeeperTakeHemopacksModSettings.HemogenInventoryLimit.ToString();
            HemogenInventoryThresholdText = RimKeeperTakeHemopacksModSettings.HemogenInventoryThreshold.ToString();
        }

        public override string SettingsCategory() => "RK TakeHemopacks";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            Rect newRect = new Rect(inRect.x, inRect.y, inRect.width / 2, inRect.height);
            listingStandard.Begin(newRect);

            listingStandard.CheckboxLabeled("Debug Log", ref RimKeeperTakeHemopacksModSettings.DebugLog, "Log Messages");
            listingStandard.Gap();

            //float x = listingStandard.GetPrivateField<float>("curX");
            //float y = listingStandard.GetPrivateField<float>("curY");
            //Texture2D HemogenpackIcon = ContentFinder<Texture2D>.Get("Things/Item/Resource/HemogenPack/HemogenPack_c");
            //GUI.DrawTexture(new Rect(x, y, 30, 30), HemogenpackIcon); //listingStandard.ButtonImage(HemogenpackIcon, 30, 30);
            //listingStandard.Gap(60);
            
            listingStandard.CheckboxLabeled("Enable shareing", ref RimKeeperTakeHemopacksModSettings.HemogenInventoryShare, "Vampire colonists will be share hemopacks");
            listingStandard.Gap();

            listingStandard.Label("How many take to inventory per stack:");
            listingStandard.IntEntry(ref RimKeeperTakeHemopacksModSettings.HemogenInventoryLimit, ref HemogenInventoryLimitText, 1);
            listingStandard.Gap();
            listingStandard.Label("Inventory threshold for start looking:");
            listingStandard.IntEntry(ref RimKeeperTakeHemopacksModSettings.HemogenInventoryThreshold, ref HemogenInventoryThresholdText, 1);
            listingStandard.Gap();

            listingStandard.End();

            Rect newRectRight = new Rect(inRect.x + (inRect.width / 2) + 20, inRect.y, inRect.width / 2, inRect.height);
            GUI.Label(new Rect(newRectRight.x, newRectRight.y, newRectRight.width, 30), "Detected hemogenpacks:");
            float cell_size = 30;
            int i = 0;
            foreach (var item in RimKeeperTakeHemopacksModSettings.hemogenPacks)
            {
                i++;
                GUI.DrawTexture(new Rect(newRectRight.x + 10, newRectRight.y + (cell_size * i), cell_size, cell_size), item.uiIcon);
                GUI.Label(new Rect(newRectRight.x + 50, newRectRight.y + (cell_size * i) + 5, newRectRight.width - cell_size, cell_size), item.label);
            }
            
            base.DoSettingsWindowContents(inRect);
        }
    }
}
