using Core40k;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Mutations40k.Mutation40kUtils;

namespace Mutations40k
{
    //Tracks the faviur for each god, meaning how much they are seen by the gods
    public class FavourTracker : IExposable
    {
        //List of the progress for each god
        private List<FavourProgress> FavourOfGods;

        //Comp that is attached to the pawn to allow all this.
        private FavourComp favourComp;

        public FavourComp FavourComp
        {
            get { return favourComp; }
        }

        //God which have the highest favour
        public FavourProgress HighestFavour => FavourOfGods.MaxBy((FavourProgress x) => x.Favour);

        public FavourTracker()
        {
            FavourOfGods = new List<FavourProgress>();
        }

        //Constructor
        public FavourTracker(FavourComp favourComp)
        {
            this.favourComp = favourComp;
            FavourOfGods = new List<FavourProgress>();
        }

        //Gets list of all favours sorted by their progress
        public IEnumerable<FavourProgress> AllFavoursSorted()
        {
            return FavourOfGods.OrderByDescending((FavourProgress x) => x.Favour);
        }

        //Gets favour level of the given god
        public GodAcceptance FavourLevelFor(ChaosGods god)
        {
            return Enumerable.FirstOrDefault(FavourOfGods, (FavourProgress x) => x.God == god)?.FavourLevel ?? GodAcceptance.Ignored;
        }

        //Gets favour value for te given god
        public float FavourValueFor(ChaosGods god)
        {
            return Enumerable.FirstOrDefault(FavourOfGods, (FavourProgress x) => x.God == god)?.Favour ?? 0f;
        }

        internal void TryAddGods(List<ChaosGods> gods)
        {
            TryAddGods(gods, FloatRange.Zero);
        }

        internal void TryAddGods(List<ChaosGods> gods, FloatRange startingRange)
        {
            IEnumerable<ChaosGods> enumerable = gods.Where((ChaosGods x) => !FavourOfGods.Any((FavourProgress w) => w.God == x));
            foreach (ChaosGods item in enumerable)
            {
                FavourOfGods.Add(new FavourProgress(item, startingRange.RandomInRange, this));
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref FavourOfGods, "FavourOfGods", LookMode.Deep);
        }
    }
}