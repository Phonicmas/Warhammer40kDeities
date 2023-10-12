using System;
using Verse;

namespace Mutations40k
{ 
    public class HediffComp_EverchangingDeficiancies : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.IsHashIntervalTick(5000))
            {
                Random rand = new Random();
                Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(Mutations40kDefOf.BEWH_TzeentchEverchangingDeficiancies);
                int maxUp = 10 - (int)hediff.Severity;
                int maxDown = maxUp - 10;
                int nextSeverity = rand.Next(maxDown, maxUp);
                while (nextSeverity == 0)
                {
                    nextSeverity = rand.Next(maxDown, maxUp);
                }
                nextSeverity = rand.Next(maxDown, maxUp);
                severityAdjustment = nextSeverity;
            }
        }
    }
}