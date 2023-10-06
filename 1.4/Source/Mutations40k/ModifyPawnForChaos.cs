using Core40k;
using Genes40k;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Mutations40k
{
    public class ModifyPawnForChaos
    {
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
            }

            foreach (Def def in giftToAdd)
            {
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

            if (!pawn.IsColonist)
            {
                return;
            }

            string letterText = "MutationGivenLetter".Translate();
            string messageText = "MutationGivenMessage".Translate(pawn.Named("PAWN"), ChaosEnumUtils.Convert(chosenGod)) + mutation;
            Find.LetterStack.ReceiveLetter(letterText, messageText, Core40kDefOf.BEWH_GiftGiven);
            ChangeFactionOpinion(true, pawn);
        }

        public static void CurseAndSmitePawn(Pawn pawn, ChaosGods chosenGod)
        {
            Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

            WeatherEvent_LightningStrike.DoStrike(pawn.Position, pawn.Map, ref boltMesh);
            Graphics.DrawMesh(boltMesh, pawn.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, 1), 0);

            pawn.health.AddHediff(Mutations40kDefOf.BEWH_GodsTemporaryCurse, null);

            if (!pawn.IsColonist)
            {
                return;
            }

            string letterText = "NoMutationGivenLetter".Translate();
            string messageText = "NoMutationGivenMessage".Translate(pawn.Named("PAWN"), ChaosEnumUtils.Convert(chosenGod));
            Find.LetterStack.ReceiveLetter(letterText, messageText, Core40kDefOf.BEWH_NoGiftGiven);
            ChangeFactionOpinion(false ,pawn);
        }

        private static void ChangeFactionOpinion(bool acceptedChaos, Pawn pawn)
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

            int goodwillChange = modSettings.opinionGainAndLossOnGift;
            HistoryEventDef chaosHistory = Mutations40kDefOf.BEWH_RejectedChaos;
            if (acceptedChaos)
            {
                chaosHistory = Mutations40kDefOf.BEWH_AcceptedChaos;
            }

            foreach (Faction faction in factionManager.AllFactionsVisible)
            {
                if (faction.Equals(pawnFaction) || faction.IsPlayer)
                {
                    continue;
                }
                if (faction.def.HasModExtension<DefModExtension_ChaosEnjoyer>())
                {
                    if (!acceptedChaos)
                    {
                        goodwillChange *= -1;
                    }
                    else if (faction.def.GetModExtension<DefModExtension_ChaosEnjoyer>().makeEnemy)
                    {
                        goodwillChange = -100 + faction.GoodwillToMakeHostile(pawnFaction);
                    }
                }
                else
                {
                    if (acceptedChaos)
                    {
                        goodwillChange *= -1;
                    }
                }
                faction.TryAffectGoodwillWith(pawnFaction, goodwillChange, false, true, chaosHistory);
                goodwillChange = modSettings.opinionGainAndLossOnGift;
            }

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
    
    }

}