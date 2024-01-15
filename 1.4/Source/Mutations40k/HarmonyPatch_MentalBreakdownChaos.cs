using Genes40k;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Mutations40k
{
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public class MentalBreakdownChaos
    {
        public static void Postfix(MentalStateDef stateDef, MentalStateHandler __instance, bool __result)
        {
            Mutations40kSettings modSettings = LoadedModManager.GetMod<Mutations40kMod>().GetSettings<Mutations40kSettings>();
            if (modSettings.disableRandomMutations)
            {
                return;
            }
            //Mental state didn't start, skip
            if (!__result)
            {
                return;
            }

            //If theres no defMod it wont give favour.
            if (!stateDef.HasModExtension<DefModExtension_MentalBreakFavoredGod>())
            {
                return;
            }

            Pawn pawn = __instance.CurState.pawn;
            //Pawn must have genes and traits
            if (pawn.genes == null || pawn.story == null)
            {
                return;
            }

            FavourComp favourComp = pawn.TryGetComp<FavourComp>();
            //If pawn cannot gain corruption, skip
            if (favourComp == null)
            {
                return;
            }

            //If uncorruptable, skip
            if (favourComp.uncorruptable)
            {
                return;
            }

            //Increases favor for each god
            DefModExtension_MentalBreakFavoredGod defMod = stateDef.GetModExtension<DefModExtension_MentalBreakFavoredGod>();

            foreach (FavourProgress item in favourComp.favourTracker.AllFavoursSorted())
            {
                float multiplier = defMod.godsFavourMultiplier.TryGetValue(item.God);
                if (multiplier > 0)
                {
                    item.TryAddProgress(100, multiplier);
                }
            }
        }
    }
}