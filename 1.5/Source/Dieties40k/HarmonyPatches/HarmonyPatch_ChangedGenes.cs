using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace Deities40k
{
    [HarmonyPatch(typeof(Pawn_GeneTracker), "Notify_GenesChanged")]
    public class ChangedGenes
    {
        public static void Postfix(Pawn_GeneTracker __instance)
        {
            DeityComp deityComp = __instance.pawn.TryGetComp<DeityComp>();

            if (deityComp == null)
            {
                return;
            }

            deityComp.UpdateValues();
        }
    }
}