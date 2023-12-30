using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace Mutations40k
{
    [HarmonyPatch(typeof(Pawn_GeneTracker), "Notify_GenesChanged")]
    public class ChangedGenes
    {
        public static void Postfix(Pawn_GeneTracker __instance)
        {
            FavourComp favourComp = __instance.pawn.TryGetComp<FavourComp>();

            if (favourComp == null)
            {
                return;
            }

            favourComp.UpdateGeneAndTraitInfo();
        }
    }
}