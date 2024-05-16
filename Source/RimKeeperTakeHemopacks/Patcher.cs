using HarmonyLib;
using Keepercraft.RimKeeperTakeHemopacks.Extensions;
using RimKeeperTakeHemopacks.Helpers;
using RimWorld;
using System.Linq;
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
            Log.Message("[RimKeeperTakeHemopacks] Patching");
            var harmony = new Harmony("Keepercraft.RimKeeperTakeHemopacks");
            harmony.PatchAll();
        }

        //[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
        [HarmonyPatch(typeof(JobGiver_TakeForInventoryStock), "TryGiveJob")]
        public static class JobGiver_TakeForInventoryStock_Patch
        {
            static ThingDef hemogenPack = DefDatabase<ThingDef>.GetNamed("HemogenPack");
            static GeneDef hemogenicGene = DefDatabase<GeneDef>.GetNamed("Hemogenic", false);

            static void Postfix(JobGiver_TakeForInventoryStock __instance, ref Job __result, Pawn pawn)
            {
                if (__result != null || pawn == null) return;
                if (!pawn.IsColonist) return;
                if (!pawn.genes.HasGene(hemogenicGene)) return;

                int pawnHemogenPackCount = pawn.inventory.Count(hemogenPack);

                DebugHelper.Message("[RimKeeperTakeHemopacks] {0} looking", pawn.Name);
                if (pawnHemogenPackCount < KeeperModSettings.HemogenInventoryThreshold)
                {
                    Thing thing = __instance.GetPrivateMethod<Thing>("FindThingFor", pawn, hemogenPack);
                    if (thing != null)
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.TakeCountToInventory, thing);
                        int b = KeeperModSettings.HemogenInventoryLimit - pawn.inventory.Count(thing.def);
                        job.count = Mathf.Min(thing.stackCount, b);
                        __result = job;
                        DebugHelper.Message("[RimKeeperTakeHemopacks] {0} try take {1}", pawn.Name, b);
                        return;
                    }

                    if (KeeperModSettings.HemogenInventoryShare)
                    {
                        foreach (var innerPawn in Find.CurrentMap.mapPawns.FreeColonists.Where(w => w != pawn))
                        {
                            DebugHelper.Message("[RimKeeperTakeHemopacks] {0} looking in {1} inventory", pawn.Name, innerPawn.Name);
                            var hemopacks = innerPawn.inventory.innerContainer.Where(w => w.def == hemogenPack);
                            if (hemopacks.Any())
                            {
                                int innerPawnHemogenPackCount = hemopacks.Sum(s => s.stackCount);
                                if (innerPawnHemogenPackCount < 2) return;
                                if (pawnHemogenPackCount > innerPawnHemogenPackCount) return;

                                int avg = (pawnHemogenPackCount + innerPawnHemogenPackCount) / 2;
                                int pawn_need_to_take = Mathf.Min(avg, KeeperModSettings.HemogenInventoryLimit);

                                Job job = JobMaker.MakeJob(JobDefOf.TakeFromOtherInventory, hemopacks.First(), innerPawn);
                                job.count = pawn_need_to_take;
                                __result = job;
                                DebugHelper.Message("[RimKeeperTakeHemopacks] {0} taking from {1} inventory {2}", pawn.Name, innerPawn.Name, pawn_need_to_take);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
