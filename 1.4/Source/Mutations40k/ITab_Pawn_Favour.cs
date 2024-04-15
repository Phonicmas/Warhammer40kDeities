using Core40k;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Noise;
using static Mutations40k.Mutation40kUtils;

namespace Mutations40k
{
    public class ITab_Pawn_Favour : ITab
    {
        public override bool IsVisible
        {
            get
            {
                Mutations40kSettings modSettings = LoadedModManager.GetMod<Mutations40kMod>().GetSettings<Mutations40kSettings>();
                if (modSettings.disableRandomMutations)
                {
                    return false;
                }

                if (SelPawn.IsHashIntervalTick(30000))
                {
                    ChangeUncorruptable();
                }

                FavourComp f = SelPawn.TryGetComp<FavourComp>();
                if (f == null || f.uncorruptable)
                {
                    return false;
                }

                return true;
            }
        }

        private void ChangeUncorruptable()
        {
            FavourComp f = SelPawn.TryGetComp<FavourComp>();
            if (f == null)
            {
                return;
            }

            if (SelPawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0)
            {
                f.uncorruptable = true;
                return;
            }

            if (SelPawn.genes != null)
            {
                foreach (Gene gene in SelPawn.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                    {
                        if (gene.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>().purity)
                        {
                            f.uncorruptable = true;
                            return;
                        }
                    }
                }
            }

            foreach (Trait trait in SelPawn.story.traits.allTraits)
            {
                if (trait.def.HasModExtension<DefModExtension_TraitAndGeneOpinion>())
                {
                    if (trait.def.GetModExtension<DefModExtension_TraitAndGeneOpinion>().purity)
                    {
                        f.uncorruptable = true;
                        return;
                    }
                }
            }

        }

        public ITab_Pawn_Favour()
        {
            size = new Vector2(300f, 200f);
            labelKey = "TabFavour".Translate();
            tutorTag = "Favour".Translate();
        }

        protected override void FillTab()
        {
            Rect position = new Rect(0f, 0f, size.x, size.y).ContractedBy(4f);
            GUI.BeginGroup(position);
            Rect rect3 = new Rect(0f, 0f, position.width, 40);
            Text.Font = GameFont.Medium;
            Widgets.Label(rect3, "GodFavour".Translate());
            Text.Font = GameFont.Small;
            Rect inRect2 = new Rect(0f, rect3.yMax, rect3.width, position.height - rect3.yMax);
            DrawCorruptionProgress(inRect2);
            GUI.EndGroup();
        }

        private float DrawCorruptionProgress(Rect inRect)
        {
            Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
            GUI.BeginGroup(inRect);
            float disBe = 4f;
            float num = 0f;
            foreach (FavourProgress godProgress in SelPawn.GetComp<FavourComp>().favourTracker.AllFavoursSorted())
            {
                Rect rect = new Rect(0f, num, inRect.width, (inRect.height/5)-disBe);
                if (godProgress.GeneAndTraitInfoGet != null && godProgress.GeneAndTraitInfoGet.wontGiveGift)
                {
                    GUI.BeginGroup(rect);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
                    Rect rectC2 = new Rect(0f, 0f, inRect.width, rect.height);
                    Widgets.Label(rectC2, "Forsaken".Translate(ChaosEnumUtils.Convert(godProgress.God)));
                    GenUI.ResetLabelAlign();
                    GUI.EndGroup();
                }
                else if (godProgress.FavourLevel < GodAcceptance.Seen)
                {
                    GUI.BeginGroup(rect);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
                    Rect rectC2 = new Rect(0f, 0f, inRect.width, rect.height);
                    Widgets.Label(rectC2, "Unknown".Translate(ChaosEnumUtils.Convert(godProgress.God)));
                    GenUI.ResetLabelAlign();
                    GUI.EndGroup();
                }
                else
                {
                    if (Mouse.IsOver(rect))
                    {
                        GUI.DrawTexture(rect, TexUI.HighlightTex);
                    }
                    GUI.BeginGroup(rect);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect rect2 = new Rect(0f, 0f, inRect.width / 2f, rect.height);
                    Widgets.Label(rect2, ChaosEnumUtils.Convert(godProgress.God).CapitalizeFirst());
                    Rect rect3 = new Rect(rect2.xMax, 0f, inRect.width / 2f, rect.height);
                    Widgets.FillableBar(rect3, godProgress.FavourPercentage, BarTex, null, doBorder: false);
                    Rect rect4 = new Rect(rect3);
                    rect4.yMin += 2f;
                    GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
                    Widgets.Label(rect4, ("WorshipStanding" + godProgress.FavourLevel).Translate());
                    GenUI.ResetLabelAlign();
                    GUI.EndGroup();
                    if (Mouse.IsOver(rect))
                    {
                        string progressDescription = GetProgressDescription(godProgress);
                        TooltipHandler.TipRegion(rect, new TipSignal(progressDescription, godProgress.God.GetHashCode() * 397945));
                    }
                }
                num += rect.height + disBe;
            }
            GUI.EndGroup();
            return num;
        }

        private string GetProgressDescription(FavourProgress progress)
        {
            StringBuilder stringBuilder = new StringBuilder();
            GodAcceptance nextLevel = progress.NextLevel;
            FavourProgress.FavorLevelThresholds.TryGetValue(nextLevel, out float value);
            value *= 100f;
            stringBuilder.AppendLine(string.Concat(new object[2]
            {
            "Level".Translate() + " ",
            (progress.FavourPercentage * 100f).ToString("N0") + "%"
            }));
            stringBuilder.AppendLine();
            if (progress.FavourLevel < GodAcceptance.Blessed)
            {
                stringBuilder.AppendLine(string.Concat(new string[2]
                {
                "NextLevel".Translate() + ": ",
                ("WorshipStanding" + nextLevel).Translate() + string.Concat(" (", value.ToString("N0") + "%", ")")
                }));
            }
            return stringBuilder.ToString();
        }
    }
}