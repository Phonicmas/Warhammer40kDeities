using Core40k;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    public class ChoiceLetter_AcceptChaosNoChoice : ChoiceLetter
    {
        public List<Def> giftsToAdd;

        public Pawn targetedPawn;

        public ChaosGods chosenGod;

        public bool? acceptedChaos;

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
                    if (acceptedChaos.HasValue)
                    {
                        if (acceptedChaos.Value)
                        {
                            ModifyPawnForChaos.ModifyPawn(giftsToAdd, targetedPawn, chosenGod);
                        }
                        else
                        {
                            ModifyPawnForChaos.CurseAndSmitePawn(targetedPawn, chosenGod);
                        }
                    }
                    Find.LetterStack.RemoveLetter(this);
                };
                diaOption.resolveTree = true;
                yield return diaOption;
            }
        }
    }

}