using RimWorld;
using Verse;

namespace Deities40k
{
    public class ThoughtWorker_SituationalThoughtMutation : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (!def.HasModExtension<DefModExtension_SituationalThought>())
            {
                return false;
            }
            else
            {
                DefModExtension_SituationalThought temp = def.GetModExtension<DefModExtension_SituationalThought>();
                if (other.genes != null)
                {
                    foreach (Gene gene in other.genes.GenesListForReading)
                    {
                        if (gene.def.HasModExtension<DefModExtension_SituationalThought>() && gene.def == temp.geneActivator)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

}