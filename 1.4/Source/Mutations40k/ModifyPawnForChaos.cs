using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Mutations40k
{
    public class ModifyPawnForChaos
    {
        public List<GeneDef> genesToAdd;

        public Pawn targetedPawn;

        public static void ModifyPawnGenes(List<GeneDef> genesToAdd, Pawn pawn)
        {
            pawn.MentalState.RecoverFromState();

            pawn.health.AddHediff(Mutations40kDefOf.BEWH_GodsTemporaryPower, null);

            if (pawn.genes == null)
            {
                return;
            }
            foreach (GeneDef gene in genesToAdd)
            {
                if (!pawn.genes.HasGene(gene))
                {
                    pawn.genes.AddGene(gene, true);
                }
            }
            return;
        }

        public static void CurseAndSmitePawn(Pawn pawn)
        {
            Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

            WeatherEvent_LightningStrike.DoStrike(pawn.Position, pawn.Map, ref boltMesh);
            Graphics.DrawMesh(boltMesh, pawn.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, 1), 0);

            pawn.health.AddHediff(Mutations40kDefOf.BEWH_GodsTemporaryCurse, null);
        }

    }

}