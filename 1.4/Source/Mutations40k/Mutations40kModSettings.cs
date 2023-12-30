using Verse;


namespace Mutations40k
{
    public class Mutations40kSettings : ModSettings
    {
        public float baseChanceForGiftOffer = 50f;

        public int maxGiftsWhenGiven = 1;

        public int opinionGainAndLossOnGift = 1;

        public int offsetPerHatedOrLovedTrait = 3;

        public int offsetPerHatedOrLovedGene = 2;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref baseChanceForGiftOffer, "baseChanceForGiftOffer", 50f);
            Scribe_Values.Look(ref maxGiftsWhenGiven, "maxGiftsWhenGiven", 1);
            Scribe_Values.Look(ref opinionGainAndLossOnGift, "opinionGainAndLossOnGift", 1);
            Scribe_Values.Look(ref offsetPerHatedOrLovedTrait, "offsetPerHatedOrLovedTrait", 3);
            Scribe_Values.Look(ref offsetPerHatedOrLovedGene, "offsetPerHatedOrLovedGene", 2);
            base.ExposeData();
        }
    }
}