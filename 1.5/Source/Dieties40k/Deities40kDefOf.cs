using RimWorld;
using Verse;

namespace Deities40k
{
    [DefOf]
    public static class Deities40kDefOf
    {

        public static DamageDef BEWH_BitingTongueDamage;

        public static GeneDef BEWH_KhorneBoilingBlood;

        public static GeneDef BEWH_KhorneMonstrousHands;

        public static HediffDef BEWH_TzeentchEverchangingDeficiancies;

        public static MentalStateDef BEWH_KhornateHungerBeserk;

        public static TraitDef BEWH_Everchosen;
        public static TraitDef BEWH_BlessingOfKhorne;
        public static TraitDef BEWH_BlessingOfNurgle;
        public static TraitDef BEWH_BlessingOfTzeentch;
        public static TraitDef BEWH_BlessingOfSlaanesh;

        public static HistoryEventDef BEWH_FactionDeityMatch;

        public static DeityDef BEWH_DeityEmperorOfMankind;

        public static DeityDef BEWH_DeityUndivided;
        public static DeityDef BEWH_DeityKhorne;
        public static DeityDef BEWH_DeityTzeentch;
        public static DeityDef BEWH_DeityNurgle;
        public static DeityDef BEWH_DeitySlaanesh;

        static Deities40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Deities40kDefOf));
        }
    }
}