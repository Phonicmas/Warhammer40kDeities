using RimWorld;
using Verse;
using Verse.AI;

namespace Mutations40k
{
    public class MentalState_KhornateHungerBerserk : MentalState
    {
        public override bool ForceHostileTo(Thing t)
        {
            return true;
        }

        public override bool ForceHostileTo(Faction f)
        {
            return true;
        }

        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }

        public override void PostEnd()
        {
            pawn.needs?.TryGetNeed<Need_KhornateHunger>()?.SetLevel(0.05f);
            base.PostEnd();
        }
    }
}