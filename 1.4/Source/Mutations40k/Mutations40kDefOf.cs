using RimWorld;
using Verse;

namespace Mutations40k
{
    [DefOf]
    public static class Mutations40kDefOf
    {

        public static DamageDef BEWH_BitingTongueDamage;

        public static GeneDef BEWH_KhorneBoilingBlood;

        public static GeneDef BEWH_KhorneMonstrousHands;

        public static HediffDef BEWH_GodsTemporaryPower;
        public static HediffDef BEWH_GodsTemporaryCurse;

        public static HediffDef BEWH_TzeentchEverchangingDeficiancies;

        public static MentalStateDef BEWH_KhornateHungerBeserk;

        public static TraitDef BEWH_Everchosen;
        public static TraitDef BEWH_BlessingOfKhorne;
        public static TraitDef BEWH_BlessingOfNurgle;
        public static TraitDef BEWH_BlessingOfTzeentch;
        public static TraitDef BEWH_BlessingOfSlaanesh;

        public static HistoryEventDef BEWH_AcceptedChaos;
        public static HistoryEventDef BEWH_RejectedChaos;

        static Mutations40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Mutations40kDefOf));
        }
    }
}