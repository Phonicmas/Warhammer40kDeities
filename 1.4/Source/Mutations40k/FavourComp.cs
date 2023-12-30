using Core40k;
using System;
using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
	public class FavourComp : ThingComp
	{
		public Pawn Pawn => parent as Pawn;

		public bool uncorruptable = false;

		public FavourTracker favourTracker;

		private int tickInterval = 60000;

		public FavourCompProperties Props => props as FavourCompProperties;

		public FavourComp()
		{
			favourTracker = new FavourTracker(this);
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			List<ChaosGods> godsToAdd = new List<ChaosGods>
			{
				ChaosGods.Khorne,
				ChaosGods.Tzeentch,
				ChaosGods.Nurgle,
				ChaosGods.Slaanesh,
				ChaosGods.Undivided
			};
			favourTracker.TryAddGods(godsToAdd);
        }

		public void UpdateGeneAndTraitInfo()
		{
			foreach (FavourProgress favourProgress in favourTracker.AllFavoursSorted())
			{
				favourProgress.UpdateGeneAndTraitInfo(Pawn);
			}
		}

		public override void CompTick()
		{
			TryGiveGift();
			PassivelyLoseFavor();
		}

		private void PassivelyLoseFavor()
		{
            if (!Pawn.IsHashIntervalTick(60000))
            {
                return;
            }
            foreach (FavourProgress favourProgress in favourTracker.AllFavoursSorted())
            {
				favourProgress.Deteriorate();
            }
        }

		private void TryGiveGift()
		{
            if (!Pawn.IsHashIntervalTick(tickInterval))
            {
                return;
            }
            Random rand = new Random();
            tickInterval = rand.Next(90000, 180000);
            foreach (FavourProgress favourProgress in favourTracker.AllFavoursSorted())
            {
                favourProgress.TryGiveGift();
            }
        }

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look(ref favourTracker, "favourTracker", this);
		}
	}
}