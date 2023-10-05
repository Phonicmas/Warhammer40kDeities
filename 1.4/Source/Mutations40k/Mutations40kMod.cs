using HarmonyLib;
using UnityEngine;
using Verse;


namespace Mutations40k
{
    public class Mutations40kMod : Mod
    {
        public static Harmony harmony;

        readonly Mutations40kSettings settings;

        public Mutations40kMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Mutations40kSettings>();
            harmony = new Harmony("Chaos40k.Mod");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            
            listingStandard.CheckboxLabeled("HasMutationChoice".Translate(), ref settings.hasMutationChoice);

            listingStandard.Label("baseChanceForGiftAcceptance".Translate(settings.baseChanceForGiftAcceptance));
            settings.baseChanceForGiftAcceptance = (int)listingStandard.Slider(settings.baseChanceForGiftAcceptance, 0, 100);

            listingStandard.Label("baseChanceForGiftOffer".Translate(settings.baseChanceForGiftOffer));
            settings.baseChanceForGiftOffer = (int)listingStandard.Slider(settings.baseChanceForGiftOffer, 0, 100);

            listingStandard.Label("maxGiftsWhenGiven".Translate(settings.maxGiftsWhenGiven));
            settings.maxGiftsWhenGiven = (int)listingStandard.Slider(settings.maxGiftsWhenGiven, 0, 10);

            listingStandard.Label("opinionGainAndLossOnGift".Translate(settings.opinionGainAndLossOnGift));
            settings.opinionGainAndLossOnGift = (int)listingStandard.Slider(settings.opinionGainAndLossOnGift, 0, 100);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ModSettingsNameMutations".Translate();
        }

    }
}