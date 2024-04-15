using Core40k;
using System;
using System.Collections.Generic;
using Verse;
using Random = System.Random;

namespace Mutations40k
{
	public class FavourComp : ThingComp
	{
		private Pawn pawn = null;

		public Pawn Pawn
		{
			get
			{
				if (pawn == null)
				{
                    if (parent is Pawn pawn2)
                    {
                        pawn = pawn2;
                    }
                }
				return pawn;
			}
		}

		public bool uncorruptable = false;

		public FavourTracker favourTracker;

		private int tickInterval = 1;

		private Mutations40kSettings modsettings;

		private Mutations40kSettings Modsettings
        {
			get
			{
				if (modsettings == null)
				{
                    modsettings = LoadedModManager.GetMod<Mutations40kMod>().GetSettings<Mutations40kSettings>();
                }
				return modsettings;
			}
		}

        public FavourCompProperties Props => props as FavourCompProperties;

		public FavourComp()
		{
			favourTracker = new FavourTracker(this);
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
            InitializeFavourTracker();
        }

		public void UpdateGeneAndTraitInfo()
		{
			if (Pawn == null)
			{
				return;
			}
            if (favourTracker == null)
            {
                InitializeFavourTracker();
            }
            foreach (FavourProgress favourProgress in favourTracker.AllFavoursSorted())
			{
				favourProgress.UpdateGeneAndTraitInfo();
			}
		}

		public override void CompTick()
		{
			if (Modsettings.disableRandomMutations)
			{
				return;
			}
			if (Pawn == null)
			{
				return;
			}
			if (uncorruptable)
			{
				return;
			}
			TryGiveGift();
			PassivelyLoseFavor();
		}

		private void PassivelyLoseFavor()
		{
			if (Pawn == null)
			{
				return;
			}
            if (!Pawn.IsHashIntervalTick(60000))
            {
                return;
            }
            if (favourTracker == null)
            {
                InitializeFavourTracker();
            }
            foreach (FavourProgress favourProgress in favourTracker.AllFavoursSorted())
            {
				favourProgress.Deteriorate();
				if (favourProgress.GeneAndTraitInfoGet.wontGiveGift && favourProgress.Favour > 0)
				{
					favourProgress.Favour = 0;
				}
            }
        }

		private void TryGiveGift()
		{
			if (Pawn == null)
			{
				return;
			}
            if (!Pawn.IsHashIntervalTick(tickInterval))
            {
                return;
            }
            if (favourTracker == null)
            {
				InitializeFavourTracker();
            }
            Random rand = new Random();
            tickInterval = rand.Next(Mutation40kUtils.ModSettings.ticksBetweenGifts.min, Mutation40kUtils.ModSettings.ticksBetweenGifts.max+1);
            foreach (FavourProgress favourProgress in favourTracker.AllFavoursSorted())
            {
                favourProgress.TryGiveGift();
            }
        }

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look(ref favourTracker, "favourTracker", this);
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref uncorruptable, "uncorruptable", false);
            Scribe_Values.Look(ref tickInterval, "tickInterval", 30000);
        }

		private void InitializeFavourTracker()
		{
            favourTracker = new FavourTracker(this);
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
	}
}