using LudeonTK;
using Verse;

namespace Deities40k
{
    //If a faction has this defMod without the makeEnemy set to true, they like chaos and those who accepts its gifts
    public static class DebugActionsDeities
    {
        [DebugAction("Warhammer", null, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 2000)]
        private static void GiveDeityProgress(Pawn p)
        {
            if (p.HasComp<DeityComp>())
            {
                DeityComp deityComp = p.GetComp<DeityComp>();
                deityComp.DevProgressGainLoss(1000);
            }
        }

        [DebugAction("Warhammer", null, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 2000)]
        private static void LoseDeityProgress(Pawn p)
        {
            if (p.HasComp<DeityComp>())
            {
                DeityComp deityComp = p.GetComp<DeityComp>();
                deityComp.DevProgressGainLoss(-1000);
            }
        }
    }

}