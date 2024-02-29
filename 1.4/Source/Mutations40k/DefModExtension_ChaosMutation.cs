using Core40k;
using RimWorld;
using System.Collections.Generic;
using Verse;
using static Mutations40k.Mutation40kUtils;

namespace Mutations40k
{
    //This DefModExtension should be given to GeneDef, to allow said gene to be considered in the pool of genes given on mental breaks.
    public class DefModExtension_ChaosMutation : DefModExtension
    {
        //List of chaos gods that gives said gene when dealing with them
        public List<ChaosGods> givenBy = new List<ChaosGods>();

        //Gene or trait is beneficial for the pawn
        public bool isBeneficial = false;

        //Weight of gene to be given, 1 is standard, if it should be more common increase it, if less decrease it
        //NOTE: If one gene has a really high one like 10 it will get pick quite often compared to all other
        public float selectionWeight = 1;

        //Gene or trait that removes all current detrimental gifts
        public bool removesDetrimentalGifts = false;

        //This should be used if the gene requires some sort of skill to be available.
        public List<WorkTags> requiredWorkTags;

        public GodAcceptance requiredAcceptance = GodAcceptance.Seen;

        public bool isConsideredMutation = false;
    }

}