using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    public class RecipeWorkerClass_Purify : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
			FavourComp favourComp = billDoer.TryGetComp<FavourComp>();
			if (favourComp == null)
			{
				return;
			}

			foreach (FavourProgress favourProgress in favourComp.favourTracker.AllFavoursSorted())
			{
                favourProgress.RemoveFavour(500);
            }
        }
    }
}