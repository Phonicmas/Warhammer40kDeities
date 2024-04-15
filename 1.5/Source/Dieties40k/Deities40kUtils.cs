using Core40k;
using Genes40k;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Deities40k
{
    public class Deities40kUtils
    {
        public enum GodAcceptance
        {
            Ignored,
            Seen,
            Acknowledged,
            Favoured,
            Blessed
        }

        private static Deities40kSettings modSettings = null;

        public static Deities40kSettings ModSettings
        {
            get
            {
                if (modSettings == null)
                {
                    modSettings = LoadedModManager.GetMod<Deities40kMod>().GetSettings<Deities40kSettings>();
                }
                return modSettings;
            }
        }

        private static bool HasRequirementsForGift(Pawn pawn, DefModExtension_DeityGift defMod)
        {
            if (!defMod.requiredGenesToGet.NullOrEmpty())
            {
                foreach (GeneDef geneDef in defMod.requiredGenesToGet)
                {
                    if (!pawn.genes.HasGene(geneDef))
                    {
                        return false;
                    }
                }
            }
            if (defMod.requiredTraitsToGet.NullOrEmpty())
            {
                foreach (TraitDef traitDef in defMod.requiredTraitsToGet)
                {
                    if (!pawn.story.traits.HasTrait(traitDef))
                    {
                        return false;
                    }
                }
            }
            if (!defMod.requiredWorkTags.NullOrEmpty())
            {
                foreach (WorkTags workTag in defMod.requiredWorkTags)
                {
                    if (pawn.WorkTagIsDisabled(workTag))
                    {
                        return false;
                    }
                }
            }
            DeityComp dComp = pawn.TryGetComp<DeityComp>();
            if (dComp != null && dComp.deityTracker.currentlySelected.FavourLevel != defMod.requiredAcceptance)
            {
                return false;
            }
            return true;
        }

        public static List<Def> GetGiftBasedOfGod(DeityDef chosenGod, Pawn pawn, bool onlyBeneficial)
        {
            WeightedSelection<Def> defSelection = new WeightedSelection<Def>();

            foreach (GeneDef geneDef in chosenGod.genesGiftable)
            {
                if (geneDef.HasModExtension<DefModExtension_DeityGift>())
                {
                    DefModExtension_DeityGift defMod = geneDef.GetModExtension<DefModExtension_DeityGift>();
                    if (!HasRequirementsForGift(pawn, defMod))
                    {
                        continue;
                    }
                    if (!pawn.genes.HasGene(geneDef))
                    {
                        if (!onlyBeneficial)
                        {
                            defSelection.AddEntry(geneDef, defMod.selectionWeight);
                        }
                        else if (defMod.isBeneficial)
                        {
                            defSelection.AddEntry(geneDef, defMod.selectionWeight);
                        }
                    }
                }
            }

            foreach (TraitDef traitDef in chosenGod.traitsGiftable)
            {
                if (traitDef.HasModExtension<DefModExtension_DeityGift>())
                {
                    DefModExtension_DeityGift defMod = traitDef.GetModExtension<DefModExtension_DeityGift>();
                    if (!pawn.story.traits.HasTrait(traitDef))
                    {
                        if (!HasRequirementsForGift(pawn, defMod))
                        {
                            continue;
                        }
                        if (!onlyBeneficial)
                        {
                            defSelection.AddEntry(traitDef, defMod.selectionWeight);
                        }
                        else if (defMod.isBeneficial)
                        {
                            defSelection.AddEntry(traitDef, defMod.selectionWeight);
                        }
                    }
                }
            }

            foreach (HediffDef hediffDef in chosenGod.hediffsGiftable)
            {
                if (hediffDef.HasModExtension<DefModExtension_DeityGift>())
                {
                    DefModExtension_DeityGift defMod = hediffDef.GetModExtension<DefModExtension_DeityGift>();
                    if (!HasRequirementsForGift(pawn, defMod))
                    {
                        continue;
                    }
                    if (!onlyBeneficial)
                    {
                        defSelection.AddEntry(hediffDef, defMod.selectionWeight);
                    }
                    else if (defMod.isBeneficial)
                    {
                        defSelection.AddEntry(hediffDef, defMod.selectionWeight);
                    }
                }
            }

            foreach (AbilityDef abilityDef in chosenGod.abilitiesGiftable)
            {
                if (abilityDef.HasModExtension<DefModExtension_DeityGift>())
                {
                    DefModExtension_DeityGift defMod = abilityDef.GetModExtension<DefModExtension_DeityGift>();
                    if (!HasRequirementsForGift(pawn, defMod))
                    {
                        continue;
                    }
                    if (!onlyBeneficial)
                    {
                        defSelection.AddEntry(abilityDef, defMod.selectionWeight);
                    }
                    else if (defMod.isBeneficial)
                    {
                        defSelection.AddEntry(abilityDef, defMod.selectionWeight);
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

        public static void ChangeFactionOpinion(Pawn pawn, DeityDef deity)
        {
            if (modSettings.opinionGainOnGift == 0)
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
                if (faction.Equals(pawnFaction) || faction.IsPlayer || !faction.def.HasModExtension<DefModExtension_FactionDeity>())
                {
                    continue;
                }

                DefModExtension_FactionDeity temp = faction.def.GetModExtension<DefModExtension_FactionDeity>();

                if (!temp.followsDeities.Contains(deity))
                {
                    continue;
                }

                HistoryEventDef chaosHistory = Deities40kDefOf.BEWH_FactionDeityMatch;
                int goodwillChange = modSettings.opinionGainOnGift;
                faction.TryAffectGoodwillWith(pawnFaction, goodwillChange, false, true, chaosHistory);
            }
        }

        public static void ModifyPawn(List<Def> giftToAdd, Pawn pawn, DeityDef chosenGod)
        {
            if (!pawn.IsColonist)
            {
                return;
            }
            if (pawn.genes == null || pawn.story == null || pawn.story.traits == null)
            {
                Log.Message("Tried to modify a pawn whom cannot be modified - Mutations 40k - Show to phonicmas in his discord if it shows");
                return;
            }

            if (pawn.InMentalState)
            {
                pawn.MentalState.RecoverFromState();
            }

            string mutation = "\n\n";

            bool removeDetrimentalAfter = false;
            foreach (Def def in giftToAdd)
            {
                if (giftToAdd.Contains(Genes40kDefOf.BEWH_DaemonMutation))
                {
                    giftToAdd.AddRange(AddDaemonParts(chosenGod, pawn));
                }
                if (def.HasModExtension<DefModExtension_DeityGift>())
                {
                    DefModExtension_DeityGift temp = def.GetModExtension<DefModExtension_DeityGift>();
                    if (temp.removesDetrimentalGifts)
                    {
                        removeDetrimentalAfter = true;
                    }
                    if (temp.newXenoIcon != null)
                    {
                        pawn.genes.iconDef = temp.newXenoIcon;
                    }
                    if (temp.newXenoName != "")
                    {
                        pawn.genes.xenotypeName = temp.newXenoName;
                    }
                }
                if (def.GetType() == typeof(GeneDef))
                {
                    pawn.genes.AddGene((GeneDef)def, true);
                    mutation += "- " + (def as GeneDef).label.CapitalizeFirst() + "\n";
                }
                else if (def.GetType() == typeof(TraitDef))
                {
                    pawn.story.traits.GainTrait(new Trait((TraitDef)def));
                    mutation += "- " + (def as TraitDef).degreeDatas[0].label.CapitalizeFirst() + "\n";
                }
                else if (def.GetType() == typeof(HediffDef))
                {
                    pawn.health.AddHediff(def as HediffDef);
                    mutation += "- " + (def as HediffDef).label.CapitalizeFirst() + "\n";
                }
                else if (def.GetType() == typeof(AbilityDef))
                {
                    pawn.abilities.GainAbility(def as AbilityDef);
                    mutation += "- " + (def as AbilityDef).label.CapitalizeFirst() + "\n";
                }
            }

            if (removeDetrimentalAfter)
            {
                List<Gene> genesToRemove = pawn.genes.GenesListForReading.FindAll(x => x.def.HasModExtension<DefModExtension_DeityGift>() && !x.def.GetModExtension<DefModExtension_DeityGift>().isBeneficial);
                if (!genesToRemove.NullOrEmpty())
                {
                    foreach (Gene gene in genesToRemove)
                    {
                        pawn.genes.RemoveGene(gene);
                    }
                }
            }

            string letterText = chosenGod.label.Translate();
            string messageText = "GiftLetterMessage".Translate(pawn.Named("PAWN"), chosenGod.label) + "GiftLetterMessageGiven".Translate(pawn.Named("PAWN")) + mutation;
            Find.LetterStack.ReceiveLetter(letterText, messageText, Core40kDefOf.BEWH_GiftGiven);
        }

        private static List<GeneDef> AddDaemonParts(DeityDef chosenGod, Pawn pawn)
        {
            Dictionary<DaemonParts, List<GeneDef>> map = new Dictionary<DaemonParts, List<GeneDef>>
            {
                { DaemonParts.Hide, new List<GeneDef>() },
                { DaemonParts.Wings, new List<GeneDef>() },
                { DaemonParts.Tail, new List<GeneDef>() },
                { DaemonParts.Horns, new List<GeneDef>() },
                { DaemonParts.Color, new List<GeneDef>() }
            };

            List<GeneDef> allDefs = (List<GeneDef>)DefDatabase<GeneDef>.AllDefs;

            foreach (GeneDef geneDef in allDefs)
            {
                if (geneDef.HasModExtension<DefModExtension_DaemonMutation>())
                {
                    DefModExtension_DaemonMutation defMod = geneDef.GetModExtension<DefModExtension_DaemonMutation>();
                    if (defMod.godGiver.Contains(chosenGod))
                    {
                        List<GeneDef> temp = map.TryGetValue(defMod.daemonPart);
                        temp.Add(geneDef);
                        map.SetOrAdd(defMod.daemonPart, temp);
                    }
                }
            }

            List<GeneDef> daemonMutationsResult = new List<GeneDef>();

            foreach (KeyValuePair<DaemonParts, List<GeneDef>> k in map)
            {
                daemonMutationsResult.Add(k.Value.RandomElement());
            }

            return daemonMutationsResult;
        }

        public static int GetOpinionBasedOnPsysens(Pawn pawn)
        {
            if (pawn == null)
            {
                return 0;
            }
            else
            {
                return (int)(pawn.GetStatValue(StatDefOf.PsychicSensitivity)/50);
            }
        }

        public static int GetOpinionBasedOnSkills(Pawn pawn, DeityDef chosenGod)
        {
            float skillsScaleAmount = 0.25f;
            float opinion = 0;
            if (!chosenGod.prefferedSkills.NullOrEmpty() && pawn != null)
            {
                SkillRecord skillRecord;
                foreach (SkillDef skill in chosenGod.prefferedSkills)
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