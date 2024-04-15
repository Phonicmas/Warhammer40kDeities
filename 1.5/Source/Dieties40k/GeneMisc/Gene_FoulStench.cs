using RimWorld;
using Verse;
using Verse.AI;

namespace Deities40k
{
    public class Gene_FoulStench : Gene
    {

        public override void Tick()
        {
            if (!pawn.IsHashIntervalTick(2500))
            {
                return;
            }
            Log.Message("Here1");
            if (pawn.Spawned)
            {
                foreach (Pawn p in pawn.Map.mapPawns.AllPawnsSpawned)
                {
                    if (!p.RaceProps.Humanlike || p == null || p == pawn || p.CurJobDef == JobDefOf.Vomit)
                    {
                        continue;
                    }
                    if (p.Position.DistanceTo(pawn.Position) < 20)
                    {
                        p.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Vomit, 500), JobCondition.InterruptForced, null, resumeCurJobAfterwards: true);
                    }
                }
            }
            base.Tick();
        }
    }
}