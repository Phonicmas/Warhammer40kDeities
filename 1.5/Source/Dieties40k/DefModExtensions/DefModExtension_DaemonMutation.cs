using Core40k;
using System.Collections.Generic;
using Verse;

namespace Deities40k
{
    public class DefModExtension_DaemonMutation : DefModExtension
    {
        public DaemonParts daemonPart = DaemonParts.None;

        public List<DeityDef> godGiver = new List<DeityDef>();
    }
}