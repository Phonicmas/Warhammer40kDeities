using System.Collections.Generic;
using Verse;

namespace Deities40k
{
    //This DefModExtension should be given to GeneDef, to allow said gene to be considered in the pool of genes given on mental breaks.
    public class DefModExtension_FactionDeity : DefModExtension
    {
        public List<DeityDef> followsDeities = new List<DeityDef>();
    }
}