using Verse;


namespace Mutations40k
{
    public class Mutations40kSettings : ModSettings
    {
        public float baseChanceForGiftOffer = 0f;

        public int maxGiftsWhenGiven = 1;

        public int opinionGainAndLossOnGift = 1;

        public int offsetPerHatedOrLovedTrait = 3;

        public int offsetPerHatedOrLovedGene = 2;

        public bool disableRandomMutations = false;

        public IntRange ticksBetweenGifts = new IntRange(600000, 1080000);

        public override void ExposeData()
        {
            Scribe_Values.Look(ref baseChanceForGiftOffer, "baseChanceForGiftOffer", 0f);
            Scribe_Values.Look(ref maxGiftsWhenGiven, "maxGiftsWhenGiven", 1);
            Scribe_Values.Look(ref opinionGainAndLossOnGift, "opinionGainAndLossOnGift", 1);
            Scribe_Values.Look(ref offsetPerHatedOrLovedTrait, "offsetPerHatedOrLovedTrait", 3);
            Scribe_Values.Look(ref offsetPerHatedOrLovedGene, "offsetPerHatedOrLovedGene", 2);
            Scribe_Values.Look(ref disableRandomMutations, "disableRandomMutations", false);
            int value = ticksBetweenGifts.max;
            int value2 = ticksBetweenGifts.min;
            Scribe_Values.Look(ref value, "ticksBetweenGiftsMax", 1080000);
            Scribe_Values.Look(ref value2, "ticksBetweenGiftsMin", 600000);
            ticksBetweenGifts = new IntRange(value2, value);
            base.ExposeData();
        }
    }
}