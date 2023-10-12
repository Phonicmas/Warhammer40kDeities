using Core40k;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static Core40k.Core40kUtils;

namespace Mutations40k
{
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public class MentalBreakdownChaos
    {
        public static void Postfix(MentalStateDef stateDef, MentalStateHandler __instance, bool __result)
        {

            if (!__result)
            {
                return;
            }

            Pawn pawn = __instance.CurState.pawn;

            if (pawn.genes == null || pawn.story == null)
            {
                return;
            }

            (Dictionary<ChaosGods, GeneAndTraitInfo>, bool) geneAndTraitInfo = GetGeneAndTraitInfo(pawn);

            if (geneAndTraitInfo.Item2)
            {
                return;
            }

            Random rand = new Random();

            //Base chance for gift 
            if (rand.Next(0, 100) > Mutation40kUtils.ModSettings.baseChanceForGiftOffer)
            {
                return;
            }

            if (!stateDef.HasModExtension<DefModExtension_MentalBreakFavoredGod>())
            {
                return;
            }

            ChaosGods chosenGod = GetGodForDealOffer(geneAndTraitInfo.Item1, stateDef, pawn);


            if (chosenGod == ChaosGods.None)
            {
                return;
            }

            List<Def> giftsToAdd = Mutation40kUtils.GetGiftBasedOfGod(chosenGod, pawn, geneAndTraitInfo.Item1.TryGetValue(chosenGod).willGiveBeneficial);
            if (giftsToAdd.NullOrEmpty())
            {
                return;
            }

            //If pawn is not colonis, do it autonomous.
            if (!pawn.IsColonist)
            {
                float nonColonistChance = 0;
                if (pawn.Faction.def.HasModExtension<DefModExtension_ChaosEnjoyer>())
                {
                    if (pawn.Faction.def.GetModExtension<DefModExtension_ChaosEnjoyer>().makeEnemy)
                    {
                        nonColonistChance = -60;
                    }
                    else
                    {
                        nonColonistChance = 15;
                    }
                }

                if (Mutation40kUtils.WillPawnAcceptChaos(geneAndTraitInfo.Item1, Mutation40kUtils.ModSettings.baseChanceForGiftOffer + nonColonistChance, chosenGod, pawn))
                {
                    ModifyPawnForChaos.ModifyPawn(giftsToAdd, pawn, chosenGod);

                }
                else
                {
                    ModifyPawnForChaos.CurseAndSmitePawn(pawn, chosenGod);
                }
                return;
            }

            Mutation40kUtils.SendMutationLetter(pawn, giftsToAdd, chosenGod, geneAndTraitInfo.Item1);
        }

        private static ChaosGods GetGodForDealOffer(Dictionary<ChaosGods, GeneAndTraitInfo> opinion, MentalStateDef stateDef, Pawn pawn)
        {
            //Standard assignment of values are 0, this way i don't have to patch every MentalStateDef, only those i want to be able to cause god attraction.
            DefModExtension_MentalBreakFavoredGod defModExtension = stateDef?.GetModExtension<DefModExtension_MentalBreakFavoredGod>();

            float? khorneMultiplier = defModExtension.khorneChance ?? 0;
            float? slaaneshMultiplier = defModExtension.slaaneshChance ?? 0;
            float? tzeentchMultiplier = defModExtension.tzeentchChance ?? 0;
            float? nurgleMultiplier = defModExtension.nurgleChance ?? 0;
            float? undividedMultiplier = defModExtension.undividedChance ?? 0;

            if (khorneMultiplier + slaaneshMultiplier + tzeentchMultiplier + nurgleMultiplier + undividedMultiplier == 0)
            {
                return ChaosGods.None;
            }

            Dictionary<ChaosGods, float> allGods = new Dictionary<ChaosGods, float>
            {
                { ChaosGods.Khorne, 1 },
                { ChaosGods.Tzeentch, 1 },
                { ChaosGods.Nurgle, 1 },
                { ChaosGods.Slaanesh, 1 },
                { ChaosGods.Undivided, 1 }
            };

            foreach (KeyValuePair<ChaosGods, GeneAndTraitInfo> dict in opinion)
            {
                if (dict.Value.wontGiveGift)
                {
                    allGods.SetOrAdd(dict.Key, -99999);
                }
                else
                {
                    int combinedOpinion = dict.Value.opinionTrait + dict.Value.opinionGene;

                    combinedOpinion = (int)GetOpinionBasedOnSkills(combinedOpinion, pawn, ChaosEnumUtils.GetGodAssociatedSkills(dict.Key), 1);

                    allGods.SetOrAdd(dict.Key, combinedOpinion);
                }
            }

            WeightedSelection<ChaosGods> godSelection = new WeightedSelection<ChaosGods>();

            godSelection.AddEntry(ChaosGods.Khorne, (double)(allGods.TryGetValue(ChaosGods.Khorne) * khorneMultiplier));
            godSelection.AddEntry(ChaosGods.Slaanesh, (double)(allGods.TryGetValue(ChaosGods.Slaanesh) * slaaneshMultiplier));
            godSelection.AddEntry(ChaosGods.Tzeentch, (double)(allGods.TryGetValue(ChaosGods.Tzeentch) * tzeentchMultiplier));
            godSelection.AddEntry(ChaosGods.Nurgle, (double)(allGods.TryGetValue(ChaosGods.Nurgle) * nurgleMultiplier));
            godSelection.AddEntry(ChaosGods.Undivided, (double)(allGods.TryGetValue(ChaosGods.Undivided) * undividedMultiplier));

            ChaosGods chosenGod = godSelection.GetRandom();

            return chosenGod;
        }

    }
}