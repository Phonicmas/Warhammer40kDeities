using Core40k;
using System;
using System.Collections.Generic;
using Verse;
using static Core40k.Core40kUtils;

namespace Mutations40k
{
    public class RecipeWorkerClass_Ritual : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            if (recipe.GetModExtension<DefModExtension_Ritual>() == null)
            {
                UnansweredCall(billDoer, ChaosGods.None, false);
                return;
            }

            DefModExtension_Ritual defMod = recipe.GetModExtension<DefModExtension_Ritual>();
            ChaosGods giftGiver = defMod.giftGiver;

            (Dictionary<ChaosGods, GeneAndTraitInfo>, bool) geneAndTraitInfo = GetGeneAndTraitInfo(billDoer);

            //Pawn is pure
            if (geneAndTraitInfo.Item2)
            {
                UnansweredCall(billDoer, giftGiver, true);
                return;
            }

            //Gift giver wont give pawn gift
            if (geneAndTraitInfo.Item1.TryGetValue(giftGiver).wontGiveGift)
            {
                UnansweredCall(billDoer, giftGiver, false);
                return;
            }

            Random rand = new Random();
            Core40kSettings modSettings = LoadedModManager.GetMod<Core40kMod>().GetSettings<Core40kSettings>();

            float chance = defMod.baseChance;
            chance += rand.Next(-1 * modSettings.randomChanceRitualNegative, modSettings.randomChanceRitualPositive);
            chance = GetOpinionBasedOnTraitsAndGenes(chance, giftGiver, geneAndTraitInfo.Item1);
            chance = GetOpinionBasedOnSkills(chance, billDoer, ChaosEnumUtils.GetGodAssociatedSkills(giftGiver), defMod.skillsScaleAmount);
            chance = (float)Math.Round(chance);

            int randNum = rand.Next(100);
            if (!(randNum <= chance))
            {
                UnansweredCall(billDoer, giftGiver, false);
                return;
            }

            List<Def> giftsToGive = Mutation40kUtils.GetGiftBasedOfGod(giftGiver, billDoer, geneAndTraitInfo.Item1.TryGetValue(giftGiver).willGiveBeneficial);

            ModifyPawnForChaos.ModifyPawn(giftsToGive, billDoer, giftGiver);
        }
    }
}