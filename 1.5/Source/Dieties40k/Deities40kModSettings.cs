using Verse;


namespace Deities40k
{
    public class Deities40kSettings : ModSettings
    {
        public int mentalBreakMultiplier = 1;

        public int opinionGainOnGift = 1;

        public float baseChanceForGiftOffer = 0f;

        public int maxGiftsWhenGiven = 1;

        public bool disableDeities = false;

        public IntRange ticksBetweenGifts = new IntRange(600000, 1080000);

        public override void ExposeData()
        {
            Scribe_Values.Look(ref baseChanceForGiftOffer, "baseChanceForGiftOffer", 0f);
            Scribe_Values.Look(ref maxGiftsWhenGiven, "maxGiftsWhenGiven", 1);
            Scribe_Values.Look(ref mentalBreakMultiplier, "mentalBreakMultiplier", 1);
            Scribe_Values.Look(ref opinionGainOnGift, "opinionGainOnGift", 1);
            Scribe_Values.Look(ref disableDeities, "disableDeities", false);
            int value = ticksBetweenGifts.max;
            int value2 = ticksBetweenGifts.min;
            Scribe_Values.Look(ref value, "ticksBetweenGiftsMax", 1080000);
            Scribe_Values.Look(ref value2, "ticksBetweenGiftsMin", 600000);
            ticksBetweenGifts = new IntRange(value2, value);
            base.ExposeData();
        }
    }
}