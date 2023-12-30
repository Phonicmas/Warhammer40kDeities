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

        public GeneAndTraitInfo geneAndTraitInfo;

        public Pawn OwnerPawn
        {
            get => geneAndTraitInfo.ownerPawn;
        }

        public int OverallOpinion
        {
            get
            {
                int opinion = GetOpinionBasedOnTraitsAndGenes(geneAndTraitInfo) + GetOpinionBasedOnPsysens(OwnerPawn);
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

        public FavourProgress(ChaosGods god)
        {
            God = god;
        }

        public FavourProgress(ChaosGods god, float value)
        {
            God = god;
            favourValue = value;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref God, "God");
            Scribe_Values.Look(ref favourValue, "value", 0f);
        }

        //Chance of god giving gift
        public bool WillGiveGift()
        {
            if (geneAndTraitInfo.wontGiveGift)
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
            if (rand.Next(0, 100) < chance)
            {
                return true;
            }
            return false;
        }

        //Tries to give gift
        public bool TryGiveGift()
        {
            if (!WillGiveGift())
            {
                return false;
            }
            List<Def> gifts = GetGiftBasedOfGod(God, OwnerPawn, geneAndTraitInfo.willGiveBeneficial);
            if (gifts == null)
            {
                return false;
            }
            ModifyPawn(gifts, OwnerPawn, God);
            ChangeFactionOpinion(OwnerPawn);
            return true;
        }

        //Tries to add progress to the god
        public bool TryAddProgress(float change, float multiplier, Pawn pawn)
        {
            UpdateGeneAndTraitInfo(pawn);
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

        public void UpdateGeneAndTraitInfo(Pawn pawn)
        {
            geneAndTraitInfo = GetGeneAndTraitInfo(pawn, God);
        }
    }
}