using RimWorld;
using Verse;

namespace Mutations40k
{
    [DefOf]
    public static class Mutations40kDefOf
    {

        public static HediffDef BEWH_GodsTemporaryPower;
        public static HediffDef BEWH_GodsTemporaryCurse;

        static Mutations40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Mutations40kDefOf));
        }
    }
}