using RimWorld;
using Verse;

namespace Deities40k
{
    public class CompProperties_BileSpew : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public EffecterDef effecterDef;

        public CompProperties_BileSpew()
        {
            compClass = typeof(CompAbilityEffect_BileSpew);
        }
    }
}