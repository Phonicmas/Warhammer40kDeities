﻿using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;


namespace Deities40k
{
    public class Deities40kMod : Mod
    {
        public static Harmony harmony;

        readonly Deities40kSettings settings;

        public Deities40kMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Deities40kSettings>();
            harmony = new Harmony("Chaos40k.Mod");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);

            listingStandard.Label("opinionGainOnGift".Translate(settings.opinionGainOnGift));
            settings.opinionGainOnGift = (int)listingStandard.Slider(settings.opinionGainOnGift, 0, 10);

            listingStandard.Label("baseChanceForGiftOffer".Translate(settings.baseChanceForGiftOffer));
            settings.baseChanceForGiftOffer = (int)listingStandard.Slider(settings.baseChanceForGiftOffer, 0, 100);

            listingStandard.Label("maxGiftsWhenGiven".Translate(settings.maxGiftsWhenGiven));
            settings.maxGiftsWhenGiven = (int)listingStandard.Slider(settings.maxGiftsWhenGiven, 0, 10);

            listingStandard.Label("mentalBreakMultiplier".Translate(settings.mentalBreakMultiplier));
            settings.mentalBreakMultiplier = (int)listingStandard.Slider(settings.mentalBreakMultiplier, 0, 10);

            listingStandard.Label(settings.ticksBetweenGifts.min.ToStringTicksToPeriodVerbose(allowHours: false) + " - " + settings.ticksBetweenGifts.max.ToStringTicksToPeriodVerbose(allowHours: false));
            listingStandard.IntRange(ref settings.ticksBetweenGifts, 2500, 7200000);

            listingStandard.CheckboxLabeled("disableDeities".Translate(), ref settings.disableDeities);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ModSettingsNameMutations".Translate();
        }

    }
}