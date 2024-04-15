using RimWorld;
using Verse;

namespace Deities40k
{
    public class ThoughtWorker_Precept_Mutation_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (otherPawn.genes != null)
            {
                int count = 0;
                foreach (Gene gene in otherPawn.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_DeityGift>() && gene.def.GetModExtension<DefModExtension_DeityGift>().isConsideredMutation)
                    {
                        count += 1;
                    }
                }

                if (count == 0)
                {
                    return ThoughtState.ActiveAtStage(0);
                }
                if (count == 1)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
                else if (count < 5)
                {
                    return ThoughtState.ActiveAtStage(2);
                }
                else if (count >= 5)
                {
                    return ThoughtState.ActiveAtStage(3);
                }

            }
            return false;
        }
    }

}