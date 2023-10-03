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

            string letterTitle = ChaosEnumToString.GetLetterTitle(chosenGod);
            string letterMessage = "ChaosAttraction".Translate(pawn.NameShortColored, ChaosEnumToString.Convert(chosenGod));

            if (!modSettings.hasMutationChoice)
            {
                bool acceptedChaos = WillPawnAcceptChaos(pawn, modSettings.baseChanceForGiftAcceptance, chosenGod);
                if (acceptedChaos)
                {
                    letterMessage += "PawnAcceptedMutation".Translate(pawn, ChaosEnumToString.Convert(chosenGod));
                }
                else
                {
                    letterMessage += "PawnRejectedMutation".Translate(pawn, ChaosEnumToString.Convert(chosenGod));
                }
                HasNoChoiceMutate(pawn, geneToAdd, letterTitle, letterMessage, acceptedChaos);
            }
            else 
            {
                letterMessage += "QueryAcceptance".Translate(pawn, ChaosEnumToString.Convert(chosenGod));
                HasChoiceMutate(pawn, geneToAdd, letterTitle, letterMessage);
            }

            return;
        }

        private static void HasNoChoiceMutate(Pawn pawn, GeneDef geneToAdd, string letterTitle, string letterMessage, bool acceptedChaos)
        {
            ChoiceLetter_AcceptChaosNoChoice letter = new ChoiceLetter_AcceptChaosNoChoice() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, genesToAdd = new List<GeneDef> { geneToAdd }, acceptedChaos = acceptedChaos };
            
            letter.OpenLetter();
        }

        private static void HasChoiceMutate(Pawn pawn, GeneDef geneToAdd, string letterTitle, string letterMessage)
        {
            ChoiceLetter_AcceptChaos letter = new ChoiceLetter_AcceptChaos() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, genesToAdd = new List<GeneDef>{ geneToAdd } };

            letter.OpenLetter();
        }
    
        private static ChaosGods GetGodForDealOffer(Pawn pawn, MentalStateDef stateDef)
        {
            //Checking traits
            if (pawn.story.traits == null || pawn.genes == null)
            {
                return ChaosGods.None;
            }
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
                        int a = 0;
                        if (trait.Degree != 0)
                        {
                            a = temp.opinionDegreeForTraitDegrees[i].TryGetValue(trait.Degree);
                        }
                        else
                        {
                            a = temp.opinionDegree[i];
                        }
                        allGods.SetOrAdd(temp.godOpinion[i], allGods.TryGetValue(temp.godOpinion[i]) + a);
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
                        allGods.SetOrAdd(temp.godOpinion[i], allGods.TryGetValue(temp.godOpinion[i]) + temp.opinionDegree[i]);
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
                if (!pawn.genes.HasGene(geneDef))
                {
                    geneSelection.AddEntry(geneDef, geneDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
                }
            }

            return geneSelection.GetRandom();
        }

        private static bool WillPawnAcceptChaos(Pawn pawn, float baseAcceptance, ChaosGods chosenGod)
        {
            int addedChance = 0;
            //Checking traits
            List<Trait> pawnTraits = pawn.story.traits.allTraits;
            foreach (Trait trait in pawnTraits)
            {
                if (trait.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                {
                    DefModExtension_TraitAndGeneOpinion temp = trait.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>();
                    for (int i = 0; i < temp.godOpinion.Count; i++)
                    {
                        if (temp.godOpinion[i] == chosenGod)
                        {
                            int a = 0;
                            if (trait.Degree != 0)
                            {
                                a = temp.opinionDegreeForTraitDegrees[i].TryGetValue(trait.Degree);
                            }
                            else
                            {
                                a = temp.opinionDegree[i];
                            }
                            addedChance += a;
                            break;
                        }
                    }
                }
            }
            //Checking Genes
            List<Gene> pawnGenes = pawn.genes.GenesListForReading;
            foreach (Gene gene in pawnGenes)
            {
                if (gene.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                {
                    DefModExtension_TraitAndGeneOpinion temp = gene.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>();
                    for (int i = 0; i < temp.godOpinion.Count; i++)
                    {
                        if (temp.godOpinion[i] == chosenGod)
                        {
                            addedChance += temp.opinionDegree[i];
                            break;
                        }
                    }
                }
            }

            addedChance *= 4;
            baseAcceptance += addedChance;

            Random rand = new Random();
            int randInt = rand.Next(0, 100);

            if (randInt <= baseAcceptance)
            {
                return true;
            }

            return false;
        }
    }
}