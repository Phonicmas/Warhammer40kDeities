using Verse;

namespace Mutations40k
{
    //If a faction has this defMod without the makeEnemy set to true, they like chaos and those who accepts its gifts
    public class DefModExtension_ChaosEnjoyer : DefModExtension
    {
        //If this is set to true, said faction will become enemy to whichever faction has pawns that accept chaos, it also makes them not accept chaos.
        public bool makeEnemy = false;
    }

}