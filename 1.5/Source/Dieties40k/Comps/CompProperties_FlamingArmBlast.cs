using RimWorld;
using Verse;

namespace Deities40k
{
    public class CompProperties_FlamingArmBlast : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public EffecterDef effecterDef;

        public CompProperties_FlamingArmBlast()
        {
            compClass = typeof(CompAbilityEffect_FlamingArmBlast);
        }
    }
}