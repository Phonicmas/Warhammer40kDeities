using Core40k;
using RimWorld;
using Verse;

namespace Mutations40k
{
    public class ThoughtWorker_Precept_Mutation_Social_SpecificGod : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (otherPawn.Ideo == null)
            {
                return false;
            }

            Precept precept = otherPawn.Ideo.PreceptsListForReading.Find(x => x.def.HasModExtension<DefModExtension_PreceptSpecificGod>());
            if (precept == null)
            {
                return false;
            }

            if (otherPawn.genes != null)
            {
                ChaosGods specificGod = precept.def.GetModExtension<DefModExtension_PreceptSpecificGod>().specificGod;

                int countGod = 0;
                int countNotGod = 0;

                foreach (Gene gene in otherPawn.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_ChaosMutation>())
                    {
                        if (gene.def.GetModExtension<DefModExtension_ChaosMutation>().givenBy.Contains(specificGod))
                        {
                            countGod += 1;
                        }
                        else
                        {
                            countNotGod += 1;
                        }
                        
                    }
                }

                if (countNotGod > countGod)
                {
                    return ThoughtState.ActiveAtStage(0);
                }
                if (countGod == 0)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
                if (countGod == 1)
                {
                    return ThoughtState.ActiveAtStage(2);
                }
                else if (countGod < 5)
                {
                    return ThoughtState.ActiveAtStage(3);
                }
                else if (countGod >= 5)
                {
                    return ThoughtState.ActiveAtStage(4);
                }

            }
            return false;
        }
    }

}