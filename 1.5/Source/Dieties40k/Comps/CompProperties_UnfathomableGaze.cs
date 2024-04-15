using RimWorld;
using Verse;

namespace Deities40k
{
    public class CompProperties_UnfathomableGaze : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public int stunTicks;

        public EffecterDef effecterDef;

        public CompProperties_UnfathomableGaze()
        {
            compClass = typeof(CompAbilityEffect_UnfathomableGaze);
        }
    }
}