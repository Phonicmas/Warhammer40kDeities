using Core40k;
using Genes40k;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Core40k.Core40kUtils;

namespace Mutations40k
{
    public class Mutation40kUtils
    {
        private static Mutations40kSettings modSettings = null;

        public static Mutations40kSettings ModSettings
        {
            get
            {
                if (modSettings == null)
                {
                    modSettings = LoadedModManager.GetMod<Mutations40kMod>().GetSettings<Mutations40kSettings>();
                }
                return modSettings;
            }
        }

        public static bool WillPawnAcceptChaos(Dictionary<ChaosGods, GeneAndTraitInfo> opinion, float baseAcceptance, ChaosGods chosenGod, Pawn pawn)
        {
            Core40kSettings modSettingsCore = LoadedModManager.GetMod<Core40kMod>().GetSettings<Core40kSettings>();
            Random rand = new Random();
            float chance = 0;
            chance += rand.Next(-1 * modSettingsCore.randomChanceRitualNegative, modSettingsCore.randomChanceRitualPositive);
            chance = GetOpinionBasedOnTraitsAndGenes(chance, chosenGod, opinion);
            chance = GetOpinionBasedOnSkills(chance, pawn, ChaosEnumUtils.GetGodAssociatedSkills(chosenGod), 1);
            chance = (float)Math.Round(chance);
            baseAcceptance += chance;

            int randInt = rand.Next(0, 100);

            if (randInt <= baseAcceptance)
            {
                return true;
            }

            return false;
        }

        public static List<Def> GetGiftBasedOfGod(ChaosGods chosenGod, Pawn pawn, bool onlyBeneficial)
        {
            WeightedSelection<Def> defSelection = new WeightedSelection<Def>();

            List<GeneDef> allGenes = (List<GeneDef>)DefDatabase<GeneDef>.AllDefs;

            foreach (GeneDef geneDef in allGenes)
            {
                if (geneDef.HasModExtension<DefModExtension_ChaosMutation>())
                {
                    DefModExtension_ChaosMutation temp = geneDef.GetModExtension<DefModExtension_ChaosMutation>();
                    if (temp.givenBy.Contains(chosenGod) && !pawn.genes.HasGene(geneDef))
                    {
                        if (!onlyBeneficial)
                        {
                            defSelection.AddEntry(geneDef, geneDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
                        }
                        else if (temp.isBeneficial)
                        {
                            defSelection.AddEntry(geneDef, geneDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
                        }
                    }
                }
            }

            List<TraitDef> allTraits = (List<TraitDef>)DefDatabase<TraitDef>.AllDefs;

            foreach (TraitDef traitDef in allTraits)
            {
                if (traitDef.HasModExtension<DefModExtension_ChaosMutation>())
                {
                    if (traitDef == Mutations40kDefOf.BEWH_Everchosen)
                    {
                        if (pawn.story.traits.HasTrait(Mutations40kDefOf.BEWH_BlessingOfKhorne) && pawn.story.traits.HasTrait(Mutations40kDefOf.BEWH_BlessingOfNurgle) && pawn.story.traits.HasTrait(Mutations40kDefOf.BEWH_BlessingOfSlaanesh) && pawn.story.traits.HasTrait(Mutations40kDefOf.BEWH_BlessingOfTzeentch))
                        {
                            defSelection.AddEntry(traitDef, traitDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    DefModExtension_ChaosMutation temp = traitDef.GetModExtension<DefModExtension_ChaosMutation>();
                    if (temp.givenBy.Contains(chosenGod) && !pawn.story.traits.HasTrait(traitDef))
                    {
                        if (!onlyBeneficial)
                        {
                            defSelection.AddEntry(traitDef, traitDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
                        }
                        else if (temp.isBeneficial)
                        {
                            defSelection.AddEntry(traitDef, traitDef.GetModExtension<DefModExtension_ChaosMutation>().selectionWeight);
                        }
                    }
                }
            }

            List<Def> giftsTogive = new List<Def>();

            Random rand = new Random();
            List<int> rando = new List<int>
            {
                rand.Next(1, ModSettings.maxGiftsWhenGiven),
                rand.Next(1, ModSettings.maxGiftsWhenGiven)
            };

            for (int i = 0; i < rando.Min(); i++)
            {
                if (defSelection.NoEntriesOrNull())
                {
                    break;
                }
                giftsTogive.Add(defSelection.GetRandomUnique());
            }

            return giftsTogive;
        }

        public static void SendMutationLetter(Pawn pawn, List<Def> giftsToAdd, ChaosGods chosenGod, Dictionary<ChaosGods, GeneAndTraitInfo> opinion)
        {
            string letterTitle = ChaosEnumUtils.GetLetterTitle(chosenGod);
            string letterMessage = "ChaosAttraction".Translate(pawn.NameShortColored, ChaosEnumUtils.Convert(chosenGod));

            if (!ModSettings.hasMutationChoice)
            {
                bool acceptedChaos = WillPawnAcceptChaos(opinion, ModSettings.baseChanceForGiftAcceptance, chosenGod, pawn);
                if (acceptedChaos)
                {
                    letterMessage += "PawnAcceptedMutation".Translate(pawn, ChaosEnumUtils.Convert(chosenGod));
                }
                else
                {
                    letterMessage += "PawnRejectedMutation".Translate(pawn, ChaosEnumUtils.Convert(chosenGod));
                }
                ChoiceLetter_AcceptChaosNoChoice letter = new ChoiceLetter_AcceptChaosNoChoice() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, giftsToAdd = giftsToAdd, acceptedChaos = acceptedChaos, chosenGod = chosenGod };

                letter.OpenLetter();
            }
            else
            {
                ChoiceLetter_AcceptChaos letter = new ChoiceLetter_AcceptChaos() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, giftsToAdd = giftsToAdd, chosenGod = chosenGod };

                letter.OpenLetter();
            }
        }

    }
}