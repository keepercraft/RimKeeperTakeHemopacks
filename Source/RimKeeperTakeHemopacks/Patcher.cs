using HarmonyLib;
using Keepercraft.RimKeeperTakeHemopacks.Extensions;
using Keepercraft.RimKeeperTakeHemopacks.Helpers;
using RimWorld;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Keepercraft.RimKeeperTakeHemopacks
{
    [StaticConstructorOnStartup]
    public class Patcher
    {
        static Patcher()
        {
            DebugHelper.SetHeader(MethodBase.GetCurrentMethod().DeclaringType.Namespace.Split('.').LastOrDefault());
            DebugHelper.Message("Patching");
            var harmony = new Harmony(MethodBase.GetCurrentMethod().DeclaringType.Namespace);
            harmony.PatchAll();
            KeeperModSettings.LoadHemopacks();
        }
    }

    //[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
    [HarmonyPatch(typeof(JobGiver_TakeForInventoryStock), "TryGiveJob")]
    public static class JobGiver_TakeForInventoryStock_Patch
    {
        static GeneDef hemogenicGene = DefDatabase<GeneDef>.GetNamed("Hemogenic", false);

        static void Postfix(JobGiver_TakeForInventoryStock __instance, ref Job __result, Pawn pawn)
        {
            if (__result != null || pawn == null) return;
            if (!pawn.IsColonist) return;
            if (!pawn.genes.HasGene(hemogenicGene)) return;

            int pawnHemogenPackCount = pawn.inventory.innerContainer.Where(c => KeeperModSettings.hemogenPacks.Contains(c.def)).Sum(s => s.stackCount);
            if (pawnHemogenPackCount < KeeperModSettings.HemogenInventoryThreshold)
            {
                DebugHelper.Message("{0} looking", pawn.Name);
                Thing thing = KeeperModSettings.hemogenPacks.Select(s => __instance.GetPrivateMethod<Thing>("FindThingFor", pawn, s)).FirstOrDefault(w => w != null);
                if (thing != null)
                {
                    Job job = JobMaker.MakeJob(JobDefOf.TakeCountToInventory, thing);
                    int b = KeeperModSettings.HemogenInventoryLimit - pawnHemogenPackCount;
                    job.count = Mathf.Min(thing.stackCount, b);
                    __result = job;
                    DebugHelper.Message("{0} try take {1}", pawn.Name, b);
                    return;
                }

                if (KeeperModSettings.HemogenInventoryShare)
                {
                    foreach (var innerPawn in Find.CurrentMap.mapPawns.FreeColonists.Where(w => w != pawn))
                    {
                        DebugHelper.Message("{0} looking in {1} inventory", pawn.Name, innerPawn.Name);
                        foreach (var hemogenPack in KeeperModSettings.hemogenPacks)
                        {
                            var hemopacks = innerPawn.inventory.innerContainer.Where(c => hemogenPack == c.def);
                            if (hemopacks.Any())
                            {
                                int innerPawnHemogenPackCount = hemopacks.Sum(s => s.stackCount);
                                if (innerPawnHemogenPackCount < 2) return;
                                if (pawnHemogenPackCount > innerPawnHemogenPackCount) return;

                                int avg = (pawnHemogenPackCount + innerPawnHemogenPackCount) / 2;
                                int pawn_need_to_take = Mathf.Min(avg, KeeperModSettings.HemogenInventoryLimit) - pawnHemogenPackCount;
                                if (pawn_need_to_take <= 0) return;

                                Job job = JobMaker.MakeJob(JobDefOf.TakeFromOtherInventory, hemopacks.First(), innerPawn);
                                job.count = pawn_need_to_take;
                                __result = job;
                                DebugHelper.Message("{0} taking from {1} inventory {2}", pawn.Name, innerPawn.Name, pawn_need_to_take);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
