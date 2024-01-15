using Genes40k;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace Mutations40k
{
	public class FavourWorldTracker : WorldComponent
	{

		public FavourWorldTracker(World world)
		: base(world)
		{
		}

        public override void FinalizeInit()
		{
			foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.race != null && x.race.Humanlike))
			{
                
				bool hasComp = false;
				foreach (CompProperties comp in def.comps)
				{
					if (comp.GetType() == typeof(FavourCompProperties))
					{
						hasComp = true;
						break;
					}
				}

				if (!hasComp)
				{
                    FavourCompProperties compProperties_Favour = new FavourCompProperties();
                    def.comps.Add(compProperties_Favour);
                }
                
				if (!def.inspectorTabs.Contains(typeof(ITab_Pawn_Favour)))
				{
                    def.inspectorTabs.Add(typeof(ITab_Pawn_Favour));
                }
				def.ResolveReferences();
			}
		}
	}
}