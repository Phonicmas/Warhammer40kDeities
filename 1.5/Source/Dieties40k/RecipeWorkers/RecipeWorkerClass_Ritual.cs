using Core40k;
using System.Collections.Generic;
using Verse;

namespace Deities40k
{
    public class RecipeWorkerClass_Ritual : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
			DeityComp deityComp = billDoer.TryGetComp<DeityComp>();
			if (deityComp == null)
			{
				return;
			}

			DefModExtension_Ritual defMod = recipe.GetModExtension<DefModExtension_Ritual>();

			foreach (DeityProgress deityProgress in deityComp.deityTracker.AllFavoursSorted())
			{
				if (deityProgress.God == deityComp.deityTracker.currentlySelected.God)
				{
                    deityProgress.TryAddProgress(50, 1);
					break;
				}
			}

        }
    }
}