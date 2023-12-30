using Core40k;
using System.Collections.Generic;
using Verse;

namespace Mutations40k
{
    //This DefModExtension should be applied on MentalStateDefs, as a multiplier for how much favour they give.
    public class DefModExtension_MentalBreakFavoredGod : DefModExtension
    {
        //Setting any of these to 0 will prevent said god from gaining favour upon triggering
        //Should not be set to below 0
        public Dictionary<ChaosGods, float> godsFavourMultiplier = new Dictionary<ChaosGods, float>
            {
                { ChaosGods.Khorne, 1 },
                { ChaosGods.Nurgle, 1 },
                { ChaosGods.Slaanesh, 1 },
                { ChaosGods.Tzeentch, 1 },
                { ChaosGods.Undivided, 1 }
            };
    }

}