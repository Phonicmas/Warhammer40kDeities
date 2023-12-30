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

        public enum ChaosAcceptance
        {
            None,
            Accepted,
            Ignore
        }

        public enum GodAcceptance
        {
            Ignored,
            Seen,
            Acknowledged,
            Favoured,
            Blessed
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

        public static void ChangeFactionOpinion(Pawn pawn)
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
                if (faction.Equals(pawnFaction) || faction.IsPlayer || !faction.def.HasModExtension<DefModExtension_ChaosEnjoyer>())
                {
                    continue;
                }

                HistoryEventDef chaosHistory = Mutations40kDefOf.BEWH_AcceptedChaos;
                int goodwillChange = modSettings.opinionGainAndLossOnGift;
                faction.TryAffectGoodwillWith(pawnFaction, goodwillChange, false, true, chaosHistory);
            }

        }


        public static List<GeneDef> daemonMutations;

        public static void ModifyPawn(List<Def> giftToAdd, Pawn pawn, ChaosGods chosenGod)
        {
            if (pawn.genes == null || pawn.story == null)
            {
                return;
            }

            if (pawn.InMentalState)
            {
                pawn.MentalState.RecoverFromState();
            }

            pawn.health.AddHediff(Mutations40kDefOf.BEWH_GodsTemporaryPower, null);

            string mutation = "\n\n";

            if (giftToAdd.Contains(Genes40kDefOf.BEWH_DaemonMutation))
            {
                giftToAdd.AddRange(AddDaemonParts(chosenGod, pawn));
                switch (chosenGod)
                {
                    case ChaosGods.Khorne:
                        if (!pawn.genes.HasGene(Genes40kDefOf.BEWH_KhorneMark))
                        {
                            giftToAdd.Add(Genes40kDefOf.BEWH_KhorneMark);
                        }
                        pawn.genes.iconDef = Genes40kDefOf.BEWH_DPKhorneIcon;
                        pawn.genes.xenotypeName = "Daemon prince of Khorne";
                        break;
                    case ChaosGods.Tzeentch:
                        if (!pawn.genes.HasGene(Genes40kDefOf.BEWH_TzeentchMark))
                        {
                            giftToAdd.Add(Genes40kDefOf.BEWH_TzeentchMark);
                        }
                        pawn.genes.iconDef = Genes40kDefOf.BEWH_DPTzeentchIcon;
                        pawn.genes.xenotypeName = "Daemon prince of Tzeentch";
                        break;
                    case ChaosGods.Slaanesh:
                        if (!pawn.genes.HasGene(Genes40kDefOf.BEWH_SlaaneshMark))
                        {
                            giftToAdd.Add(Genes40kDefOf.BEWH_SlaaneshMark);
                        }
                        pawn.genes.iconDef = Genes40kDefOf.BEWH_DPSlaaneshIcon;
                        pawn.genes.xenotypeName = "Daemon prince of Slaanesh";
                        break;
                    case ChaosGods.Nurgle:
                        if (!pawn.genes.HasGene(Genes40kDefOf.BEWH_NurgleMark))
                        {
                            giftToAdd.Add(Genes40kDefOf.BEWH_NurgleMark);
                        }
                        pawn.genes.iconDef = Genes40kDefOf.BEWH_DPNurgleIcon;
                        pawn.genes.xenotypeName = "Daemon prince of Nurgle";
                        break;
                    case ChaosGods.Undivided:
                        if (!pawn.genes.HasGene(Genes40kDefOf.BEWH_UndividedMark))
                        {
                            giftToAdd.Add(Genes40kDefOf.BEWH_UndividedMark);
                        }
                        pawn.genes.iconDef = Genes40kDefOf.BEWH_DPUndividedIcon;
                        pawn.genes.xenotypeName = "Daemon prince of the Undivided";
                        break;
                    default:
                        break;
                }
            }

            bool removeDetrimentalAfter = false;
            foreach (Def def in giftToAdd)
            {
                if (def.HasModExtension<DefModExtension_ChaosMutation>() && def.GetModExtension<DefModExtension_ChaosMutation>().removesDetrimentalGifts)
                {
                    removeDetrimentalAfter = true;
                }
                if (def.GetType() == typeof(GeneDef))
                {
                    mutation += "- " + (def as GeneDef).label.CapitalizeFirst() + "\n";
                    pawn.genes.AddGene((GeneDef)def, true);
                }
                else if (def.GetType() == typeof(TraitDef))
                {
                    pawn.story.traits.GainTrait(new Trait((TraitDef)def));
                    mutation += "- " + (def as TraitDef).degreeDatas[0].label.CapitalizeFirst() + "\n";
                }
            }

            if (removeDetrimentalAfter)
            {
                List<Gene> genesToRemove = pawn.genes.GenesListForReading.FindAll(x => x.def.HasModExtension<DefModExtension_ChaosMutation>() && !x.def.GetModExtension<DefModExtension_ChaosMutation>().isBeneficial);
                foreach (Gene gene in genesToRemove)
                {
                    pawn.genes.RemoveGene(gene);
                }
            }

            if (!pawn.IsColonist)
            {
                return;
            }

            string letterText = ChaosEnumUtils.GetLetterTitle(chosenGod).Translate();
            string messageText = "GiftLetterMessage".Translate(pawn.Named("PAWN"), ChaosEnumUtils.Convert(chosenGod)) + "GiftLetterMessageGiven".Translate(pawn.Named("PAWN")) + mutation;
            Find.LetterStack.ReceiveLetter(letterText, messageText, Core40kDefOf.BEWH_GiftGiven);
        }

        private static List<GeneDef> AddDaemonParts(ChaosGods chosenGod, Pawn pawn)
        {
            Dictionary<DaemonParts, List<GeneDef>> map = new Dictionary<DaemonParts, List<GeneDef>>
            {
                { DaemonParts.Hide, new List<GeneDef>() },
                { DaemonParts.Wings, new List<GeneDef>() },
                { DaemonParts.Tail, new List<GeneDef>() },
                { DaemonParts.Horns, new List<GeneDef>() },
                { DaemonParts.Color, new List<GeneDef>() }
            };

            if (daemonMutations.NullOrEmpty())
            {
                daemonMutations = new List<GeneDef>();
                List<GeneDef> temp = (List<GeneDef>)DefDatabase<GeneDef>.AllDefs;
                foreach (GeneDef geneDef in temp)
                {
                    if (geneDef.HasModExtension<DefModExtension_DaemonMutation>())
                    {
                        daemonMutations.Add(geneDef);
                    }
                }
            }

            foreach (GeneDef geneDef in daemonMutations)
            {
                DefModExtension_DaemonMutation defMod = geneDef.GetModExtension<DefModExtension_DaemonMutation>();
                if (defMod.godGiver.Contains(chosenGod))
                {
                    List<GeneDef> temp = map.TryGetValue(defMod.daemonPart);
                    temp.Add(geneDef);
                    map.SetOrAdd(defMod.daemonPart, temp);
                }
            }

            List<GeneDef> daemonMutationsResult = new List<GeneDef>();

            foreach (KeyValuePair<DaemonParts, List<GeneDef>> k in map)
            {
                daemonMutationsResult.Add(k.Value.RandomElement());
            }

            return daemonMutationsResult;
        }

        public static int GetOpinionBasedOnTraitsAndGenes(GeneAndTraitInfo geneAndTraitInfo)
        {
            int opinion = 0;
            opinion += geneAndTraitInfo.opinionTrait * ModSettings.offsetPerHatedOrLovedTrait;
            opinion += geneAndTraitInfo.opinionGene * ModSettings.offsetPerHatedOrLovedGene;

            return opinion;
        }

        public static int GetOpinionBasedOnPsysens(Pawn pawn)
        {
            return (int)(pawn.GetStatValue(StatDefOf.PsychicSensitivity) / 50);
        }

        public static int GetOpinionBasedOnSkills(Pawn pawn, List<SkillDef> skillsScale, float skillsScaleAmount = 0.25f)
        {
            float opinion = 0;
            if (!skillsScale.NullOrEmpty())
            {
                SkillRecord skillRecord;
                foreach (SkillDef skill in skillsScale)
                {
                    skillRecord = pawn.skills.GetSkill(skill);
                    if (skillRecord != null)
                    {
                        opinion += skillRecord.GetLevel() * skillsScaleAmount;
                    }
                }
            }

            return (int)opinion;
        }
    }
}