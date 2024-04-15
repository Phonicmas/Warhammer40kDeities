using HarmonyLib;
using Verse;

namespace Deities40k
{
    [HarmonyPatch(typeof(Pawn), "DoKillSideEffects")]
    public class KhornateHungerPatch
    {
        public static void Postfix(DamageInfo? dinfo, Pawn __instance)
        {
            if (dinfo.HasValue && dinfo.Value.Instigator != null && dinfo.Value.Instigator is Pawn pawn)
            {
                if (__instance.RaceProps.Humanlike)
                {
                    pawn.needs?.TryGetNeed<Need_KhornateHunger>()?.Notify_KilledPawn(dinfo);
                }
            }
        }
    }
}