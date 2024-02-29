using Core40k;
using System;
using System.Collections.Generic;
using Verse;
using static Core40k.Core40kUtils;
using static Mutations40k.Mutation40kUtils;

namespace Mutations40k
{
    public class FavourProgress : IExposable
    {
        //Progress range, min and max
        public static FloatRange ProgressRange = new FloatRange(0f, 10000f);

        //Factor of how much favour is gained
        private static readonly SimpleCurve ProgressProgression = new SimpleCurve
        {
            { 0f, 1f },
            { 1000f, 0.8f },
            { 5000f, 0.6f },
            { 8000f, 4f },
            { 9500f, 0.2f }
        };

        //Factor of how fast favour deteriorates
        private static readonly SimpleCurve ProgressDeteriorationRate = new SimpleCurve
        {
            { 0f, 0f },
            { 1000f, 0.05f },
            { 4000f, 0.5f },
            { 8000f, 1f },
            { 10000f, 2f }
        };

        public ChaosGods God;

        private float favourValue;

        private FavourTracker favourTracker;

        private GeneAndTraitInfo geneAndTraitInfo;

        private AdditionalInfoExtension additionalInfoExtension;

        public AdditionalInfoExtension GeneAndTraitInfoGet
        {
            get
            {
                if (additionalInfoExtension == null)
                {
                    UpdateGeneAndTraitInfo();
                }
                return additionalInfoExtension;
            }
            set
            {
                additionalInfoExtension = value;
            }
        }

        private Pawn ownerPawn;

        public Pawn OwnerPawn
        {
            get
            {
                if (ownerPawn == null)
                {
                    if (favourTracker != null && favourTracker.FavourComp != null)
                    {
                        ownerPawn = favourTracker.FavourComp.Pawn;
                    }
                    else if (GeneAndTraitInfoGet != null)
                    {
                        ownerPawn = GeneAndTraitInfoGet.ownerPawn;
                    }
                }
                return ownerPawn;
            }
        }

        public int OverallOpinion
        {
            get
            {
                if (OwnerPawn == null)
                {
                    Log.Error("Mutation 40k - Could not get god opinion, Null Pawn");
                    return 0;
                }
                int opinion = GetOpinionBasedOnTraitsAndGenes(GeneAndTraitInfoGet) + GetOpinionBasedOnPsysens(OwnerPawn);
                if (God == ChaosGods.Undivided)
                {
                     opinion += GetOpinionBasedOnSkills(OwnerPawn, ChaosEnumUtils.GetGodAssociatedSkills(God), 0.0625f);
                }
                else
                {
                    opinion += GetOpinionBasedOnSkills(OwnerPawn, ChaosEnumUtils.GetGodAssociatedSkills(God));
                }
                return opinion;
            }
                
        }

        //Treshold for when gods either favour a pawn more or less
        public static Dictionary<GodAcceptance, float> FavorLevelThresholds = new Dictionary<GodAcceptance, float>
        {
            {
                GodAcceptance.Ignored,
                0f
            },
            {
                GodAcceptance.Seen,
                0.05f
            },
            {
                GodAcceptance.Acknowledged,
                0.4f
            },
            {
                GodAcceptance.Favoured,
                0.8f
            },
            {
                GodAcceptance.Blessed,
                0.95f
            }
        };

        public float Favour
        {
            get
            {
                return favourValue;
            }
            set
            {
                if (value < 0)
                {
                    favourValue = 0;
                }
                else
                {
                    favourValue = value;
                } 
            }
        }

        public float FavourPercentage => favourValue / ProgressRange.max;

        //Favour level
        public GodAcceptance FavourLevel
        {
            get
            {
                if (favourValue >= ProgressRange.max * 0.95f)
                {
                    return GodAcceptance.Blessed;
                }
                if (favourValue >= ProgressRange.max * 0.8f)
                {
                    return GodAcceptance.Favoured;
                }
                if (favourValue >= ProgressRange.max * 0.4f)
                {
                    return GodAcceptance.Acknowledged;
                }
                if (favourValue >= ProgressRange.max * 0.05f)
                {
                    return GodAcceptance.Seen;
                }
                return GodAcceptance.Ignored;
            }
        }

        //Increases favour level unless it is maxed
        public GodAcceptance NextLevel => (FavourLevel == GodAcceptance.Blessed) ? GodAcceptance.Blessed : (FavourLevel + 1);

        public FavourProgress()
        {
        }

        public FavourProgress(FavourTracker favourTracker)
        {
            this.favourTracker = favourTracker;
        }

        public FavourProgress(ChaosGods god, FavourTracker favourTracker)
        {
            God = god;
            this.favourTracker = favourTracker;
        }

        public FavourProgress(ChaosGods god, float value, FavourTracker favourTracker)
        {
            God = god;
            favourValue = value;
            this.favourTracker = favourTracker;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref God, "God");
            Scribe_References.Look(ref ownerPawn, "ownerPawn");
            Scribe_Values.Look(ref favourValue, "value", 0f);
            Scribe_Deep.Look(ref additionalInfoExtension, "additionalInfoExtension", ownerPawn);
        }

        //Chance of god giving gift
        public bool WillGiveGift()
        {
            if (GeneAndTraitInfoGet.wontGiveGift)
            {
                return false;
            }
            float chance = Mutation40kUtils.ModSettings.baseChanceForGiftOffer;
            chance += OverallOpinion;
            Random rand = new Random();
            switch (FavourLevel)
            {
                case GodAcceptance.Ignored:
                    chance = -1000;
                    break;
                case GodAcceptance.Seen:
                    chance += 5;
                    break;
                case GodAcceptance.Acknowledged:
                    chance += 15;
                    break;
                case GodAcceptance.Favoured:
                    chance += 35;
                    break;
                case GodAcceptance.Blessed:
                    chance += 50;
                    break;
                default:
                    break;
            }
            int t = rand.Next(0, 100);
            if (t < chance)
            {
                return true;
            }
            return false;
        }

        //Tries to give gift
        public bool TryGiveGift()
        {
            if (OwnerPawn == null)
            {
                Log.Error("Mutation 40k - Could not give gift, Null Pawn");
                return false;
            }
            if (GeneAndTraitInfoGet == null)
            {
                Log.Error("Mutation 40k - Could not give gift, Null GeneAndTraitInfo");
                return false;
            }
            UpdateGeneAndTraitInfo();
            if (!WillGiveGift())
            {
                return false;
            }
            List<Def> gifts = GetGiftBasedOfGod(God, OwnerPawn, GeneAndTraitInfoGet.willGiveBeneficial);
            if (gifts.NullOrEmpty())
            {
                return false;
            }
            ModifyPawn(gifts, OwnerPawn, God);
            ChangeFactionOpinion(OwnerPawn);
            return true;
        }

        //Tries to add progress to the god
        public bool TryAddProgress(float change, float multiplier)
        {
            UpdateGeneAndTraitInfo();
            float num = ((change > 0f) ? (change * ProgressProgression.Evaluate(favourValue)) : change);
            favourValue = ProgressRange.ClampToRange((favourValue + num + OverallOpinion)*multiplier);
            return true;
        }

        //Tries to remove progress from the god
        public void Deteriorate(float change = 100f)
        {
            Favour -= change * ProgressDeteriorationRate.Evaluate(favourValue);
        }

        public void RemoveFavour(float change)
        {
            Favour -= change;
        }

        public void UpdateGeneAndTraitInfo()
        {
            if (OwnerPawn == null)
            {
                Log.Warning("Mutation 40k - Could not update Gene and Trait info, Null Pawn");
                return;
            }
            GeneAndTraitInfoGet = GetGeneAndTraitInfo(OwnerPawn, God);
        }
    }
}