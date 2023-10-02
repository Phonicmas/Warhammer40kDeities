using Verse;

namespace Mutations40k
{
    //This DefModExtension should be applied on MentalStateDefs, to give them unique chances for each god to give gift when triggered
    public class DefModExtension_MentalBreakFavoredGod : DefModExtension
    {
        //Setting any of these to 0 will prevent said type of mentalBreak from giving a gift from said god.
        //Should not be set to below 0
        public float? khorneChance = 1;

        public float? tzeentchChance = 1;
        
        public float? nurgleChance = 1;
        
        public float? slaaneshChance = 1;

        public float? undividedChance = 1;
    }

}