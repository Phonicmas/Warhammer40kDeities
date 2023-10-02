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

            listingStandard.Label("baseChanceForGiftOffer".Translate(settings.baseChanceForGiftOffer));
            settings.baseChanceForGiftOffer = (int)listingStandard.Slider(settings.baseChanceForGiftOffer, 0f, 100f);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ModSettingsNameChaos".Translate();
        }

    }
}