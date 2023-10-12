using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    public class Need_KhornateHunger : Need
    {
        public const float FallPerDay = 1f / 30f;

        private const float MinAgeForNeed = 13f;

        protected override bool IsFrozen
        {
            get
            {
                if ((float)pawn.ageTracker.AgeBiologicalYears < MinAgeForNeed)
                {
                    return true;
                }
                return base.IsFrozen;
            }
        }

        public override bool ShowOnNeedList
        {
            get
            {
                if ((float)pawn.ageTracker.AgeBiologicalYears < MinAgeForNeed)
                {
                    return false;
                }
                return base.ShowOnNeedList;
            }
        }

        public Need_KhornateHunger(Pawn newPawn)
            : base(newPawn)
        {
            threshPercents = new List<float> { 0.3f };
        }

        public void SetLevel(float level)
        {
            CurLevel = level;
        }

        public override void NeedInterval()
        {
            if (!IsFrozen)
            {
                CurLevel -= 8.333333E-05f;
            }
            if (CurLevel > 0 && pawn.MentalStateDef == Mutations40kDefOf.BEWH_KhornateHungerBeserk)
            {
                pawn.MentalState.RecoverFromState();
            }
            if (CurLevel == 0 && !pawn.InMentalState)
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(Mutations40kDefOf.BEWH_KhornateHungerBeserk);
            }
        }

        public void Notify_KilledPawn(DamageInfo? dinfo)
        {
            if (dinfo.HasValue && (dinfo?.WeaponBodyPartGroup != null || dinfo?.WeaponLinkedHediff != null || (dinfo.Value.Weapon != null && dinfo.Value.Weapon.IsMeleeWeapon)))
            {
                CurLevel = 1f;
            }
        }
    }
}