using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Deities40k
{
    //If a faction has this defMod without the makeEnemy set to true, they like chaos and those who accepts its gifts
    public class DeityDef : Def
    {
        //Below are things that can be gifted by the god
        public List<GeneDef> genesGiftable;
        public List<TraitDef> traitsGiftable;
        //Giftable item such as blessed armors and weapons?

        public List<HediffDef> hediffsGiftable;

        public List<AbilityDef> abilitiesGiftable;

        //Below are things that prevents the god from giving gifts
        public List<GeneDef> genesForsaken;
        public List<TraitDef> traitsForsaken;

        //Below are things that makes said god only give beneficial gifts
        public List<GeneDef> genesForcesBeneficial;
        public List<TraitDef> traitsForcesBeneficial;

        //Below are things that makes said god dislike player
        public List<GeneDef> genesDislike;
        public List<TraitDef> traitsDislike;

        //Below are things that makes said god dislike player
        public List<GeneDef> genesLike;
        public List<TraitDef> traitsLike;

        //Below are the skills that the god like
        public List<SkillDef> prefferedSkills;

        //Multiplier for favour when gained or lost
        public float favourGainMultiplier = 1f;
        public float favourLossMultiplier = 1f;

        //Tags which increases or decreases favour of selected god, they will be checked in code
        public List<string> favourIncreaseTags;
        public List<string> favourDecreaseTags;
    }

}