using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

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

            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if (trait.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                {
                    if (trait.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>().purity)
                    {
                        return;
                    }

                }
            }

            Mutations40kSettings modSettings = LoadedModManager.GetMod<Mutations40kMod>().GetSettings<Mutations40kSettings>();

            Random rand = new Random();

            //Base chance for gift 
            if (rand.Next(0, 100) > modSettings.baseChanceForGiftOffer)
            {
                return;
            }

            ChaosGods chosenGod = GetGodForDealOffer(pawn, stateDef);

            if (chosenGod == ChaosGods.None)
            {
                return;
            }

            GeneDef geneToAdd = GetGeneBasedOfGod(chosenGod, pawn);

            if (geneToAdd == null && chosenGod != ChaosGods.Undivided)
            {
                chosenGod = ChaosGods.Undivided;
                geneToAdd = GetGeneBasedOfGod(chosenGod, pawn);
                if (geneToAdd == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            string letterTitle = ChaosEnumToString.GetLetterTitle(chosenGod);
            string letterMessage = "ChaosAttraction".Translate(pawn.NameShortColored, ChaosEnumToString.Convert(chosenGod));

            /*if (!modSettings.hasMutationChoice)
            {
                Log.Message("No Choice");
                HasNoChoiceMutate(pawn, geneToAdd);
            }
            else 
            {
                Log.Message("Choice");
                HasChoiceMutate(pawn, geneToAdd);
            }*/

            return;

        }

        private static void HasNoChoiceMutate(Pawn pawn, GeneDef geneToAdd)
        {
            //Make a popup like for when they have a choice, except this one just shows the outcome without anything else
        }

        private static void HasChoiceMutate(Pawn pawn, GeneDef geneToAdd)
        {
            string letterTitle = "TzeentchLetterTitle".Translate();
            string godResponsible = "TzeentchName".Translate();
            string letterMessage = "ChaosAttraction".Translate(pawn.NameShortColored, godResponsible);

            ChoiceLetter_AcceptChaos choiceLetter_AcceptChaos = new ChoiceLetter_AcceptChaos() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, genesToAdd = { geneToAdd } };

            choiceLetter_AcceptChaos.OpenLetter();
        }
    
        private static ChaosGods GetGodForDealOffer(Pawn pawn, MentalStateDef stateDef)
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

            //Checking traits
            List<Trait> pawnTraits = pawn.story.traits.allTraits;
            foreach (Trait trait in pawnTraits)
            {
                if (trait.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                {
                    DefModExtension_TraitAndGeneOpinion temp = trait.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>();
                    if (temp.purity)
                    {
                        return ChaosGods.None;
                    }
                    for (int i = 0; i < temp.godWontGift.Count; i++)
                    {
                        allGods.SetOrAdd(temp.godWontGift[i], -99999);
                    }
                    for (int i = 0; i < temp.godOpinion.Count; i++)
                    {
                        int t = temp.opinionDegree[i];
                        if (t > 1)
                        {
                            t = 1;
                        }
                        else if (t < -1)
                        {
                            t = -1;
                        }
                        allGods.SetOrAdd(temp.godOpinion[i], allGods.TryGetValue(temp.godOpinion[i]) + t);
                    }
                }
            }

            //Checking genes
            List<Gene> pawnGenes = pawn.genes.GenesListForReading;
            foreach (Gene gene in pawnGenes)
            {
                if (gene.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                {
                    DefModExtension_TraitAndGeneOpinion temp = gene.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>();
                    if (temp.purity)
                    {
                        return ChaosGods.None;
                    }
                    for (int i = 0; i < temp.godWontGift.Count; i++)
                    {
                        allGods.SetOrAdd(temp.godWontGift[i], -99999);
                    }
                    for (int i = 0; i < temp.godOpinion.Count; i++)
                    {
                        int t = temp.opinionDegree[i];
                        if (t > 1)
                        {
                            t = 1;
                        }
                        else if (t < -1)
                        {
                            t = -1;
                        }
                        allGods.SetOrAdd(temp.godOpinion[i], allGods.TryGetValue(temp.godOpinion[i]) + t);
                    }
                }
            }
            //Checks genes and traits here and then multiply the collective by multiplyer above.

            /*Log.Message("Khorne: " + allGods.TryGetValue(ChaosGods.Khorne) + " / " + khorneMultiplier);
            Log.Message("Slaanesh: " + allGods.TryGetValue(ChaosGods.Slaanesh) + " / " + slaaneshMultiplier);
            Log.Message("Tzeentch: " + allGods.TryGetValue(ChaosGods.Tzeentch) + " / " + tzeentchMultiplier);
            Log.Message("Nurgle: " + allGods.TryGetValue(ChaosGods.Nurgle) + " / " + nurgleMultiplier);
            Log.Message("Undivided: " + allGods.TryGetValue(ChaosGods.Undivided) + " / " + undividedMultiplier);*/

            WeightedSelection<ChaosGods> godSelection = new WeightedSelection<ChaosGods>();

            godSelection.AddEntry(ChaosGods.Khorne, (double)(allGods.TryGetValue(ChaosGods.Khorne) * khorneMultiplier));
            godSelection.AddEntry(ChaosGods.Slaanesh, (double)(allGods.TryGetValue(ChaosGods.Slaanesh) * slaaneshMultiplier));
            godSelection.AddEntry(ChaosGods.Tzeentch, (double)(allGods.TryGetValue(ChaosGods.Tzeentch) * tzeentchMultiplier));
            godSelection.AddEntry(ChaosGods.Nurgle, (double)(allGods.TryGetValue(ChaosGods.Nurgle) * nurgleMultiplier));
            godSelection.AddEntry(ChaosGods.Undivided, (double)(allGods.TryGetValue(ChaosGods.Undivided) * undividedMultiplier));

            return godSelection.GetRandom();
        }

        private static GeneDef GetGeneBasedOfGod(ChaosGods chosenGod, Pawn pawn)
        {
            //Check database for genedef and find all with DefModExtension_ChaosMutation with selected god and then randomly find one.
            //Maake sure pawn does not have it already
            List<GeneDef> allGenes = (List<GeneDef>)DefDatabase<GeneDef>.AllDefs;
            List<GeneDef> possibleGenesToGive = new List<GeneDef>();

            foreach (GeneDef geneDef in allGenes)
            {
                if (geneDef.HasModExtension<DefModExtension_ChaosMutation>() && geneDef.GetModExtension<DefModExtension_ChaosMutation>().givenBy.Contains(chosenGod) && !pawn.genes.HasGene(geneDef))
                {
                    possibleGenesToGive.Add(geneDef);
                }
            }

            if (possibleGenesToGive.NullOrEmpty())
            {
                return null;
            }

            WeightedSelection<GeneDef> geneSelection = new WeightedSelection<GeneDef>();

            foreach (GeneDef geneDef in possibleGenesToGive)
            {
                geneSelection.AddEntry(geneDef, geneDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
            }

            return geneSelection.GetRandom();
        }

    
    }
}