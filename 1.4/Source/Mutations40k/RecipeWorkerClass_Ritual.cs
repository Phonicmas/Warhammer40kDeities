using Core40k;
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

			FavourComp favourComp = billDoer.TryGetComp<FavourComp>();
			if (favourComp == null)
			{
				return;
			}

			DefModExtension_Ritual defMod = recipe.GetModExtension<DefModExtension_Ritual>();
			ChaosGods giftGiver = defMod.giftGiver;

            //Is pure
			if (favourComp.uncorruptable)
			{
				UnansweredCall(billDoer, giftGiver, true);
				return;
			}

			foreach (FavourProgress favourProgress in favourComp.favourTracker.AllFavoursSorted())
			{
				if (favourProgress.God == giftGiver)
				{
					favourProgress.TryAddProgress(50, 1);
				}
			}

        }
    }
}