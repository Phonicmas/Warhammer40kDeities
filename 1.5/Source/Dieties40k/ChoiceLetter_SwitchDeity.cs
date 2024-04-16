using RimWorld;
using System.Collections.Generic;
using Verse;
using static RimWorld.IdeoFoundation_Deity;

namespace Deities40k
{
    //If a faction has this defMod without the makeEnemy set to true, they like chaos and those who accepts its gifts
    public class ChoiceLetter_SwitchDeity : ChoiceLetter
    {
        public DeityProgress currentGod;

        public DeityProgress newGod;

        public Pawn pawn;

        public override bool ShouldAutomaticallyOpenLetter => true;

        public override bool CanDismissWithRightClick => false;

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                Log.Message("Cur" + currentGod);
                Log.Message("New" + newGod);
                Log.Message("Paw" + pawn);
                DiaOption SwitchDeity = new DiaOption("SwitchDeity".Translate(newGod.God.label));
                DiaOption StayWithDeity = new DiaOption("StayWithDeity".Translate(currentGod.God.label));
                SwitchDeity.action = delegate
                {
                    DeityComp deityComp = pawn.GetComp<DeityComp>();
                    currentGod.Deteriorate(currentGod.Favour * 0.4f);
                    deityComp.deityTracker.currentlySelected = newGod;
                    Find.LetterStack.RemoveLetter(this);
                };
                SwitchDeity.resolveTree = true;
                StayWithDeity.action = delegate
                {
                    Find.LetterStack.RemoveLetter(this);
                };
                StayWithDeity.resolveTree = true;
                yield return SwitchDeity;
                yield return StayWithDeity;
            }
        }
    }

}