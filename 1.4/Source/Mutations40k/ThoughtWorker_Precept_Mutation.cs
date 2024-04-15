using RimWorld;
using Verse;

namespace Mutations40k
{
    public class ThoughtWorker_Precept_Mutation : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.genes != null)
            {
                int count = 0;
                foreach (Gene gene in p.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_ChaosMutation>() && gene.def.GetModExtension<DefModExtension_ChaosMutation>().isConsideredMutation)
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