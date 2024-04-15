using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Deities40k
{
    //Tracks the favour for each god, meaning how much they are seen by the gods
    public class DeityTracker : IExposable
    {
        public DeityTracker()
        {
            Deities = new List<DeityProgress>();
            TryAddGods(DefDatabase<DeityDef>.AllDefs);
        }

        public DeityTracker(DeityComp favourComp)
        {
            this.favourComp = favourComp;
            Deities = new List<DeityProgress>();
            TryAddGods(DefDatabase<DeityDef>.AllDefs);
            currentlySelected = Deities.Find(x => x.God == Deities40kDefOf.BEWH_DeityEmperorOfMankind);
        }

        //List of the progress for each god
        private List<DeityProgress> Deities;

        public DeityProgress currentlySelected;

        private DeityComp favourComp;

        public DeityComp FavourComp
        {
            get { return favourComp; }
        }

        //Gets list of all favours sorted by their progress
        public IEnumerable<DeityProgress> AllFavoursSorted()
        {
            return Deities.OrderByDescending((DeityProgress x) => x.Favour);
        }

        internal void TryAddGods(IEnumerable<DeityDef> gods)
        {
            TryAddGods(gods, FloatRange.Zero);
        }

        internal void TryAddGods(IEnumerable<DeityDef> gods, FloatRange startingRange)
        {
            IEnumerable<DeityDef> enumerable = gods.Where((DeityDef x) => !Deities.Any((DeityProgress w) => w.God == x));
            foreach (DeityDef item in enumerable)
            {
                Deities.Add(new DeityProgress(item, startingRange.RandomInRange, this));
            }
        }
        
        public void ExposeData()
        {
            Scribe_Collections.Look(ref Deities, "Deities", LookMode.Deep);
            Scribe_Deep.Look(ref currentlySelected, "currentlySelected", this);
        }
    }
}