using Genes40k;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Deities40k
{
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public class MentalBreakdownChaos
    {
        public static void Postfix(MentalStateDef stateDef, MentalStateHandler __instance, bool __result)
        {
            Deities40kSettings modSettings = LoadedModManager.GetMod<Deities40kMod>().GetSettings<Deities40kSettings>();
            if (modSettings.disableDeities)
            {
                return;
            }
            //Mental state didn't start, skip
            if (!__result)
            {
                return;
            }

            Pawn pawn = __instance.CurState.pawn;
            //Pawn must have genes and traits
            if (pawn.genes == null || pawn.story == null)
            {
                return;
            }

            DeityComp deityComp = pawn.TryGetComp<DeityComp>();
            //If pawn cannot gain corruption, skip
            if (deityComp == null)
            {
                return;
            }

            if (deityComp.deityTracker.currentlySelected.forsaken)
            {
                return;
            }

            foreach (DeityProgress item in deityComp.deityTracker.AllFavoursSorted())
            {
                item.TryAddProgress(100, 1);
            }
        }
    }
}