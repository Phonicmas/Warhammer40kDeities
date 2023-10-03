using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    public class ChoiceLetter_AcceptChaosNoChoice : ChoiceLetter
    {
        public List<GeneDef> genesToAdd;

        public Pawn targetedPawn;

        public bool acceptedChaos;

        public override bool CanDismissWithRightClick => false;

        public override bool CanShowInLetterStack
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                DiaOption diaOption = new DiaOption("OkChaos".Translate());
                diaOption.action = delegate
                {
                    if (acceptedChaos)
                    {
                        ModifyPawnForChaos.ModifyPawnGenes(genesToAdd, targetedPawn);
                    }
                    else
                    {
                        ModifyPawnForChaos.CurseAndSmitePawn(targetedPawn);
                    }
                    Find.LetterStack.RemoveLetter(this);
                };
                diaOption.resolveTree = true;
                yield return diaOption;
            }
        }
    }

}