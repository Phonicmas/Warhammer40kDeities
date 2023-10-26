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

        public bool? acceptedByChaos;

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
                DiaOption diaOption = new DiaOption("PleadToChaos".Translate());
                DiaOption optionReject = new DiaOption("IgnoreWishpers".Translate());
                diaOption.action = delegate
                {
                    if (acceptedByChaos.HasValue)
                    {
                        if (acceptedByChaos.Value)
                        {
                            ModifyPawnForChaos.ModifyPawn(giftsToAdd, targetedPawn, chosenGod);
                        }
                        else
                        {
                            ModifyPawnForChaos.CurseAndSmitePawn(targetedPawn, chosenGod);
                        }
                        Mutation40kUtils.ChangeFactionOpinion(true, targetedPawn);
                    }
                    else
                    {
                        string letterText = ChaosEnumUtils.GetLetterTitle(ChaosGods.None).Translate();
                        string messageText = "GiftLetterMessageUnanswered".Translate(targetedPawn.Named("PAWN"));
                        Find.LetterStack.ReceiveLetter(letterText, messageText, LetterDefOf.NeutralEvent);
                    }
                    Find.LetterStack.RemoveLetter(this);
                };
                diaOption.resolveTree = true;
                optionReject.action = delegate
                {
                    Mutation40kUtils.ChangeFactionOpinion(false, targetedPawn);
                    Find.LetterStack.RemoveLetter(this);
                };
                optionReject.resolveTree = true;
                yield return diaOption;
                yield return optionReject;
            }
        }
    }

}