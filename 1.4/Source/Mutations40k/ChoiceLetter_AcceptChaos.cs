using Core40k;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    public class ChoiceLetter_AcceptChaos : ChoiceLetter
    {
        public List<Def> giftsToAdd;

        public Pawn targetedPawn;

        public ChaosGods chosenGod;

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
                DiaOption diaOption = new DiaOption("AcceptMutation".Translate());
                DiaOption optionReject = new DiaOption("RejectMutation".Translate());
                diaOption.action = delegate
                {
                    ModifyPawnForChaos.ModifyPawn(giftsToAdd, targetedPawn, chosenGod);
                    Find.LetterStack.RemoveLetter(this);
                };
                diaOption.resolveTree = true;
                optionReject.action = delegate
                {
                    ModifyPawnForChaos.CurseAndSmitePawn(targetedPawn, chosenGod);
                    Find.LetterStack.RemoveLetter(this);
                };
                optionReject.resolveTree = true;
                yield return diaOption;
                yield return optionReject;
            }
        }
    }

}