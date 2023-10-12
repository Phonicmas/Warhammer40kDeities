using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace Mutations40k
{
    [HarmonyPatch]
    public class MonsterousHandsPatch
    {
        [HarmonyPatch(typeof(EquipmentUtility), nameof(EquipmentUtility.CanEquip), new Type[]
{
    typeof(Thing),
    typeof(Pawn),
    typeof(string),
    typeof(bool)
}, new ArgumentType[]
{
    ArgumentType.Normal,
    ArgumentType.Normal,
    ArgumentType.Out,
    ArgumentType.Normal
})]
        [HarmonyPostfix]
        public static bool CanEquip_Postfix(bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (!__result)
            {
                return __result;
            }
            if (pawn.genes != null && pawn.genes.HasGene(Mutations40kDefOf.BEWH_KhorneMonstrousHands))
            {
                if (thing.def.IsMeleeWeapon || thing.def.IsRangedWeapon)
                {
                    cantReason = "MonstrousHands".Translate(pawn.LabelShort);
                    return false;
                }
            }
            return __result;
        }
    }
}