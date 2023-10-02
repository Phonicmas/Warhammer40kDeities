using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    public class ModifyPawnForChaos
    {
        public List<GeneDef> genesToAdd;

        public Pawn targetedPawn;

        public static void modifyPawnGenes(List<GeneDef> genesToAdd, Pawn pawn)
        {
            //Remove mental state here and give them some temp boost in power
            if (pawn.genes == null)
            {
                return;
            }
            foreach (GeneDef gene in genesToAdd)
            {
                if (!pawn.genes.HasGene(gene))
                {
                    pawn.genes.AddGene(gene, true);
                }
            }
            return;
        }

        public static void curseAndSmitePawn(Pawn pawn)
        {

        }
    }

}