using RimWorld;
using Verse;

namespace Mutations40k
{
    public class CompProperties_AbilityFlamingArmBlast : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public EffecterDef effecterDef;

        public CompProperties_AbilityFlamingArmBlast()
        {
            compClass = typeof(CompAbilityEffect_FlamingArmBlast);
        }
    }
}