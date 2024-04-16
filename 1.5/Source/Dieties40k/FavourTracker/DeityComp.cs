using Verse;
using Random = System.Random;

namespace Deities40k
{
	public class DeityComp : ThingComp
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

		public DeityTracker deityTracker;

		private int tickInterval = 1;

		private Deities40kSettings modsettings;

		private Deities40kSettings Modsettings
        {
			get
			{
				if (modsettings == null)
				{
                    modsettings = LoadedModManager.GetMod<Deities40kMod>().GetSettings<Deities40kSettings>();
                }
				return modsettings;
			}
		}

        public DeityCompProperties Props => props as DeityCompProperties;

		public DeityComp()
		{
            deityTracker = new DeityTracker(this);
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
        }

		public override void CompTick()
		{
			if (Modsettings.disableDeities)
			{
				return;
			}
			if (Pawn == null)
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
            foreach (DeityProgress deityProgress in deityTracker.AllFavoursSorted())
            {
                deityProgress.Deteriorate();
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
            Random rand = new Random();
            tickInterval = rand.Next(Deities40kUtils.ModSettings.ticksBetweenGifts.min, Deities40kUtils.ModSettings.ticksBetweenGifts.max+1);
			deityTracker.currentlySelected.TryGiveGift();
        }

		public void UpdateValues()
		{
			foreach (DeityProgress deityProgress in deityTracker.AllFavoursSorted())
			{
				deityProgress.UpdateValues();
			}
		}

		public void DevProgressGainLoss(float change)
		{
			deityTracker.currentlySelected.DevProgressGainLoss(change);
		}

        public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look(ref deityTracker, "deityTracker", this);
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref tickInterval, "tickInterval", 30000);
        }
	}
}