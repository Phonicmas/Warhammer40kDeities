using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using static Deities40k.Deities40kUtils;

namespace Deities40k
{
    public class DeityProgress : IExposable
    {
        public DeityProgress()
        {
        }

        public DeityProgress(DeityTracker deityTracker)
        {
            this.deityTracker = deityTracker;
        }

        public DeityProgress(DeityDef god, DeityTracker deityTracker)
        {
            God = god;
            this.deityTracker = deityTracker;
        }

        public DeityProgress(DeityDef god, float value, DeityTracker deityTracker)
        {
            God = god;
            favourValue = value;
            this.deityTracker = deityTracker;
        }

        public static FloatRange ProgressRange = new FloatRange(0f, 10000f);
        private static readonly SimpleCurve ProgressProgression = new SimpleCurve
        {
            { 0f, 1f },
            { 1000f, 0.8f },
            { 5000f, 0.6f },
            { 8000f, 0.4f },
            { 9500f, 0.2f }
        };
        private static readonly SimpleCurve ProgressDeteriorationRate = new SimpleCurve
        {
            { 0f, 0f },
            { 1000f, 0.05f },
            { 4000f, 0.5f },
            { 8000f, 1f },
            { 10000f, 2f }
        };

        public DeityDef God;

        private readonly DeityTracker deityTracker;

        private Pawn ownerPawn;
        public Pawn OwnerPawn
        {
            get
            {
                if (ownerPawn == null)
                {
                    if (deityTracker != null && deityTracker.FavourComp != null)
                    {
                        ownerPawn = deityTracker.FavourComp.Pawn;
                    }
                }
                return ownerPawn;
            }
        }

        private float favourValue;
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
        public GodAcceptance NextLevel => (FavourLevel == GodAcceptance.Blessed) ? GodAcceptance.Blessed : (FavourLevel + 1);
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


        public bool willOnlyGiveBeneficial = false;
        public bool forsaken = false;

        public int opinion = 0;


        //Chance of god giving gift
        public bool WillGiveGift()
        {
            if (forsaken)
            {
                return false;
            }
            float chance = Deities40kUtils.ModSettings.baseChanceForGiftOffer;
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
            UpdateValues();
            if (!WillGiveGift())
            {
                return false;
            }
            List<Def> gifts = GetGiftBasedOfGod(God, OwnerPawn, willOnlyGiveBeneficial);
            if (gifts.NullOrEmpty())
            {
                return false;
            }
            ModifyPawn(gifts, OwnerPawn, God);
            ChangeFactionOpinion(OwnerPawn, God);
            return true;
        }

        public void DevProgressGainLoss(float change)
        {
            Favour += change;
        }

        //Tries to add progress to the god
        public bool TryAddProgress(float change, float multiplier)
        {
            float num = ((change > 0f) ? (change * ProgressProgression.Evaluate(favourValue)) : change);
            Favour = ProgressRange.ClampToRange((favourValue + num)* multiplier * God.favourGainMultiplier);
            return true;
        }

        //Tries to remove progress from the god
        public void Deteriorate(float change = 100f)
        {
            Favour -= change * ProgressDeteriorationRate.Evaluate(favourValue) * God.favourLossMultiplier;
            if (forsaken)
            {
                Favour = 0;
            }
        }

        public void RemoveFavour(float change)
        {
            Favour -= change;
        }

        public void UpdateValues()
        {
            if (OwnerPawn == null)
            {
                Log.Warning("Mutation 40k - Could not update Gene and Trait info, Null Pawn");
                return;
            }
            //Update forced beneficial gift
            bool flag1 = false;
            if (!God.genesForcesBeneficial.NullOrEmpty())
            {
                willOnlyGiveBeneficial = false;
                foreach (GeneDef geneDef in God.genesForcesBeneficial)
                {
                    if (OwnerPawn.genes.HasGene(geneDef))
                    {
                        willOnlyGiveBeneficial = true;
                        flag1 = true;
                    }
                }
            }
            if (!God.traitsForcesBeneficial.NullOrEmpty() && !flag1)
            {
                willOnlyGiveBeneficial = false;
                foreach (TraitDef traitDef in God.traitsForcesBeneficial)
                {
                    if (OwnerPawn.story.traits.HasTrait(traitDef))
                    {
                        willOnlyGiveBeneficial = true;
                    }
                }
            }
            //Update forsaken
            bool flag2 = false;
            if (!God.genesForsaken.NullOrEmpty())
            {
                forsaken = false;
                foreach (GeneDef geneDef in God.genesForsaken)
                {
                    if (OwnerPawn.genes.HasGene(geneDef))
                    {
                        forsaken = true;
                        flag2 = true;
                    }
                }
            }
            if (!God.traitsForsaken.NullOrEmpty() && !flag2)
            {
                forsaken = false;
                foreach (TraitDef traitDef in God.traitsForsaken)
                {
                    if (OwnerPawn.story.traits.HasTrait(traitDef))
                    {
                        forsaken = true;
                    }
                }
            }
        }

        public void UpdateOpinion()
        {
            //Calculate opinion by adding giftable genes and traits with and liked genes and traits and subtracting disliked genes and traits
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref God, "God");
            Scribe_References.Look(ref ownerPawn, "ownerPawn");
            Scribe_Values.Look(ref favourValue, "favourValue", 0f);
            Scribe_Values.Look(ref willOnlyGiveBeneficial, "willOnlyGiveBeneficial", false);
            Scribe_Values.Look(ref forsaken, "forsaken", false);
            Scribe_Values.Look(ref opinion, "opinion", 0);
        }
    }
}