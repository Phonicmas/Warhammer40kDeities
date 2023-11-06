using Core40k;
using Genes40k;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Core40k.Core40kUtils;
using static System.Collections.Specialized.BitVector32;

namespace Mutations40k
{
    public class Mutation40kUtils
    {

        public enum ChaosAcceptance
        {
            None,
            Accepted,
            //Rejected,
            Ignore
        }

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

        public static bool? PawnAndGodAcceptance(Dictionary<ChaosGods, GeneAndTraitInfo> opinion, float baseAcceptance, ChaosGods chosenGod, Pawn pawn)
        {
            if (chosenGod == ChaosGods.None)
            {
                return null;
            }
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
                    if (!temp.requiredWorkTags.NullOrEmpty())
                    {
                        foreach (WorkTags workTag in temp.requiredWorkTags)
                        {
                            if (pawn.WorkTagIsDisabled(workTag))
                            {
                                break;
                            }
                        }
                    }
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
            string letterTitle = "MentalBreakLetterTitle".Translate();
            string letterMessage = "MentalBreakLetterMessage".Translate(pawn.NameShortColored);

            //If pawn is not colonist or slave, do it autonomous.
            if (!ModSettings.hasMutationChoice)
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
                bool? acceptedChaosNullable = PawnAndGodAcceptance(opinion, ModSettings.baseChanceForGiftAcceptance + nonColonistChance, chosenGod, pawn);
                if (acceptedChaosNullable.HasValue)
                {
                    if (acceptedChaosNullable.Value)
                    {
                        letterMessage += "PawnPleadAccepted".Translate(pawn, ChaosEnumUtils.Convert(chosenGod));
                    }
                    else if (!acceptedChaosNullable.Value)
                    {
                        letterMessage += "PawnPleadRejected".Translate(pawn, ChaosEnumUtils.Convert(chosenGod));
                    }
                }
                else
                {
                    letterMessage += "PawnPleadUnanswered".Translate(pawn, ChaosEnumUtils.Convert(chosenGod));
                }

                ChoiceLetter_AcceptChaosNoChoice letter = new ChoiceLetter_AcceptChaosNoChoice() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, giftsToAdd = giftsToAdd, acceptedChaos = acceptedChaosNullable, chosenGod = chosenGod };

                letter.OpenLetter();
            }
            else
            {
                bool? acceptedChaosNullable = PawnAndGodAcceptance(opinion, ModSettings.baseChanceForGiftAcceptance, chosenGod, pawn);
                ChoiceLetter_AcceptChaos letter = new ChoiceLetter_AcceptChaos() { title = letterTitle, Text = letterMessage, targetedPawn = pawn, giftsToAdd = giftsToAdd, chosenGod = chosenGod, acceptedByChaos = acceptedChaosNullable };

                letter.OpenLetter();
            }
        }

        public static void ChangeFactionOpinion(ChaosAcceptance chaosAcceptance, Pawn pawn)
        {
            Mutations40kSettings modSettings = LoadedModManager.GetMod<Mutations40kMod>().GetSettings<Mutations40kSettings>();

            if (modSettings.opinionGainAndLossOnGift == 0)
            {
                return;
            }

            FactionManager factionManager = Find.FactionManager;
            Faction pawnFaction = pawn.Faction;

            if (pawnFaction == null)
            {
                return;
            }

            foreach (Faction faction in factionManager.AllFactionsVisible)
            {
                if (faction.Equals(pawnFaction) || faction.IsPlayer)
                {
                    continue;
                }
                HistoryEventDef chaosHistory;
                int goodwillChange = modSettings.opinionGainAndLossOnGift;

                switch (chaosAcceptance)
                {
                    case ChaosAcceptance.Ignore:
                        chaosHistory = Mutations40kDefOf.BEWH_IgnoredChaos;
                        if (faction.def.HasModExtension<DefModExtension_ChaosEnjoyer>() && !faction.def.GetModExtension<DefModExtension_ChaosEnjoyer>().makeEnemy)
                        {
                            goodwillChange *= -1;
                        }
                        break;
                    case ChaosAcceptance.Accepted:
                        chaosHistory = Mutations40kDefOf.BEWH_AcceptedChaos;
                        //If accepted then chaos enjoyers get set opnion gain, non chaos enjoyer gets set opinion loss and chaosenjoyer enemies, become instant enemies.
                        if (!faction.def.HasModExtension<DefModExtension_ChaosEnjoyer>())
                        {
                            goodwillChange *= -1;
                        }
                        else if (faction.def.GetModExtension<DefModExtension_ChaosEnjoyer>().makeEnemy)
                        {
                            goodwillChange = faction.GoodwillToMakeHostile(pawnFaction);
                        }
                        break;
                    default:
                        Log.Error("Error in ChangeFactionOption, chaosAcceptance was None or Null, report this to Phonicmas if you see it.");
                        return;
                }

                faction.TryAffectGoodwillWith(pawnFaction, goodwillChange, false, true, chaosHistory);
            }

        }

    }
}