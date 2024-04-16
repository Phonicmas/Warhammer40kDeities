using Core40k;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static Deities40k.Deities40kUtils;
using static RimWorld.IdeoFoundation_Deity;

namespace Deities40k
{
    public class ITab_Pawn_Favour : ITab
    {

        public DeityComp Deity
        {
            get
            {
                return SelPawn.TryGetComp<DeityComp>();
            }
        }

        public override bool IsVisible
        {
            get
            {
                Deities40kSettings modSettings = LoadedModManager.GetMod<Deities40kMod>().GetSettings<Deities40kSettings>();
                if (modSettings.disableDeities)
                {
                    return false;
                }

                if (Deity == null || Deity.deityTracker == null || Deity.deityTracker.AllFavoursSorted().EnumerableNullOrEmpty())
                {
                    return false;
                }
                return true;
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
            Rect window = new Rect(0f, 0f, size.x, size.y).ContractedBy(4f);
            GUI.BeginGroup(window);
            Rect rect = new Rect(0f, 0f, window.width, 30f);
            Text.Font = GameFont.Medium ;
            DeityProgress god = Deity.deityTracker.currentlySelected;
            if (Widgets.ButtonText(rect, god.God.label))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                IEnumerable<DeityProgress> deities = Deity.deityTracker.AllFavoursSorted();
                foreach (DeityProgress deityP in deities)
                {
                    if (god == deityP)
                    {
                        continue;
                    }
                    list.Add(new FloatMenuOption(deityP.God.label, delegate
                    {
                        ChangeSelectedGod(deityP);
                    }, Widgets.PlaceholderIconTex, Color.white));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            Rect inRect1 = new Rect(0f, 30f, window.width, 30f);
            Text.Font = GameFont.Small;
            if (god.forsaken)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(inRect1, "Forsaken".Translate(god.God.label));
            }
            else
            {
                Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
                if (Mouse.IsOver(inRect1))
                {
                    GUI.DrawTexture(inRect1, TexUI.HighlightTex);
                }
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.FillableBar(inRect1, god.FavourPercentage, BarTex, null, doBorder: false);
                Widgets.Label(inRect1, "CurrentStanding".Translate(("WorshipStanding" + god.FavourLevel).Translate()));
                if (Mouse.IsOver(inRect1))
                {
                    string progressDescription = GetProgressDescription(god);
                    TooltipHandler.TipRegion(inRect1, new TipSignal(progressDescription, god.God.GetHashCode() * 397945));
                }
                Rect inRect2 = new Rect(0f, inRect1.yMax, window.width / 2, window.height - (rect.yMax + inRect1.yMax));
                Rect inRect3 = new Rect(inRect2);
                inRect3.x += inRect2.xMax;
                Rect inRect4 = new Rect(inRect1);
                inRect4.y = inRect2.yMax;
                Text.Anchor = TextAnchor.UpperCenter;
                Vector2 scrollPosition1 = Vector2.zero;
                Vector2 scrollPosition2 = Vector2.zero;
                Widgets.LabelScrollable(inRect2, GetLikesDescription(god.God), ref scrollPosition1);
                Widgets.LabelScrollable(inRect3, GetDislikesDescription(god.God), ref scrollPosition2);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(inRect4, "Hover to see what increases favour");
                if (Mouse.IsOver(inRect4))
                {
                    GUI.DrawTexture(inRect4, TexUI.HighlightTex);
                    string progressDescription = GetFavourDescription(god.God);
                    TooltipHandler.TipRegion(inRect4, new TipSignal(progressDescription, god.God.GetHashCode() * 397946));
                }
            }
            GenUI.ResetLabelAlign();
            GUI.EndGroup();
        }

        private string GetLikesDescription(DeityDef god)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag1 = true;
            stringBuilder.AppendLine("Likes: ");

            //Genes Related
            List<GeneDef> genesPawnHas = new List<GeneDef>();
            List<GeneDef> likedGenes = new List<GeneDef>();
            if (!god.genesLike.NullOrEmpty())
            {
                likedGenes.AddRange(god.genesLike);
            }
            if (!god.genesGiftable.NullOrEmpty())
            {
                likedGenes.AddRange(god.genesGiftable);
            }
            if (!likedGenes.NullOrEmpty())
            {
                foreach (GeneDef geneDef in likedGenes)
                {
                    if (SelPawn.genes != null && SelPawn.genes.HasGene(geneDef))
                    {
                        genesPawnHas.Add(geneDef);
                    }
                }
            }
            if (!genesPawnHas.NullOrEmpty())
            {
                stringBuilder.AppendLine("\nPawn Genes: ");
                foreach (GeneDef geneDef in genesPawnHas)
                {
                    stringBuilder.AppendLine(geneDef.label.CapitalizeFirst());
                }
                flag1 = false;
            }

            //Trait Related
            List<TraitDef> traitsPawnHas = new List<TraitDef>();
            List<TraitDef> likedTraits = new List<TraitDef>();
            if (!god.traitsLike.NullOrEmpty())
            {
                likedTraits.AddRange(god.traitsLike);
            }
            if (!god.traitsGiftable.NullOrEmpty())
            {
                likedTraits.AddRange(god.traitsGiftable);
            }
            if (!likedTraits.NullOrEmpty())
            {
                foreach (TraitDef traitDef in likedTraits)
                {
                    if (SelPawn.story != null && SelPawn.story.traits != null && SelPawn.story.traits.HasTrait(traitDef))
                    {
                        traitsPawnHas.Add(traitDef);
                    }
                }
            }
            if (!traitsPawnHas.NullOrEmpty())
            {
                stringBuilder.AppendLine("\nPawn Traits: ");
                foreach (TraitDef traitDef in traitsPawnHas)
                {
                    stringBuilder.AppendLine(traitDef.degreeDatas.First().label.CapitalizeFirst());
                }
                flag1 = false;
            }

            if (flag1)
            {
                stringBuilder.Append("None");
            }

            return stringBuilder.ToString();
        }

        private string GetDislikesDescription(DeityDef god)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag1 = true;
            stringBuilder.AppendLine("Dislikes: ");

            //Genes Related
            List<GeneDef> genesPawnHas = new List<GeneDef>();
            List<GeneDef> dislikedGenes = new List<GeneDef>();
            if (!god.genesDislike.NullOrEmpty())
            {
                dislikedGenes.AddRange(god.genesDislike);
            }
            if (!dislikedGenes.NullOrEmpty())
            {
                foreach (GeneDef geneDef in dislikedGenes)
                {
                    if (SelPawn.genes != null && SelPawn.genes.HasGene(geneDef))
                    {
                        genesPawnHas.Add(geneDef);
                    }
                }
            }
            if (!genesPawnHas.NullOrEmpty())
            {
                stringBuilder.AppendLine("\nPawn Genes: ");
                foreach (GeneDef geneDef in genesPawnHas)
                {
                    stringBuilder.AppendLine(geneDef.label.CapitalizeFirst());
                }
                flag1 = false;
            }

            //Trait Related
            List<TraitDef> traitsPawnHas = new List<TraitDef>();
            List<TraitDef> dislikedTraits = new List<TraitDef>();
            if (!god.traitsDislike.NullOrEmpty())
            {
                dislikedTraits.AddRange(god.traitsDislike);
            }
            if (!dislikedTraits.NullOrEmpty())
            {
                foreach (TraitDef traitDef in dislikedTraits)
                {
                    if (SelPawn.story != null && SelPawn.story.traits != null && SelPawn.story.traits.HasTrait(traitDef))
                    {
                        traitsPawnHas.Add(traitDef);
                    }
                }
            }
            if (!traitsPawnHas.NullOrEmpty())
            {
                stringBuilder.AppendLine("\nPawn Traits: ");
                foreach (TraitDef traitDef in traitsPawnHas)
                {
                    stringBuilder.AppendLine(traitDef.degreeDatas.First().label.CapitalizeFirst());
                }
                flag1 = false;
            }

            if (flag1)
            {
                stringBuilder.AppendLine("None");
            }

            return stringBuilder.ToString();
        }

        private string GetFavourDescription(DeityDef God)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Favour for " + God.label + " can be gained the following ways:");
            stringBuilder.Append("Praying at a shrine");
            if (!God.favourIncreaseTags.NullOrEmpty())
            {
                foreach (string favourWay in God.favourIncreaseTags)
                {
                    stringBuilder = stringBuilder.Append(favourWay);
                }
            }
            
            return stringBuilder.ToString();
        }

        private string GetProgressDescription(DeityProgress progress)
        {
            StringBuilder stringBuilder = new StringBuilder();
            GodAcceptance nextLevel = progress.NextLevel;
            DeityProgress.FavorLevelThresholds.TryGetValue(nextLevel, out float value);
            value *= 100f;
            stringBuilder.AppendLine(string.Concat(new object[2]{"Level".Translate() + " ",(progress.FavourPercentage * 100f).ToString("N0") + "%"}));
            stringBuilder.AppendLine();
            if (progress.FavourLevel < GodAcceptance.Blessed)
            {
                stringBuilder.AppendLine(string.Concat(new string[2]{"NextLevel".Translate() + ": ",("WorshipStanding" + nextLevel).Translate() + string.Concat(" (", value.ToString("N0") + "%", ")")}));
            }
            return stringBuilder.ToString();
        }
    
        private void ChangeSelectedGod(DeityProgress deityP)
        {
            //Make prompt to ask if user really wishes to and warn that they will lose some favour
            ChoiceLetter_SwitchDeity letter = new ChoiceLetter_SwitchDeity
            {
                title = "SwitchLetterTitle".Translate(),
                newGod = deityP,
                currentGod = Deity.deityTracker.currentlySelected,
                pawn = SelPawn,
                Text = "SwitchLetterMessage".Translate(),
                def = LetterDefOf.NeutralEvent,
            };
            Find.LetterStack.ReceiveLetter(letter);
        }
    }
}