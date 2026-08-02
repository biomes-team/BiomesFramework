using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace BiomesCore.Patches.Plants
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public class Plant_PlantCollected_Patch
    {
        public static void Postfix(Pawn by, Plant __instance)
        {
			PlantHarvestExtension plantExt = __instance.def?.GetModExtension<PlantHarvestExtension>();
			if (plantExt != null)
			{
				if (plantExt.memory != null)
				{
					by?.needs?.mood?.thoughts?.memories.TryGainMemory(plantExt.memory);
				}
				if (!plantExt.extraHarvests.NullOrEmpty())
				{
					foreach (PlantHarvestExtension.ExtraHarvest extH in plantExt.extraHarvests)
					{
						if (Rand.Value > extH.harvestChance)
						{
							//Log.Message("no harvest - random");
							continue;
						}
						if (__instance.Growth < extH.harvestMinGrowth)
						{
							//Log.Message("no harvest - immature");
							continue;
						}
						if (extH.minSkillLevel > 0 && (by.skills == null || by.skills.GetSkill(SkillDefOf.Plants).GetLevel() < extH.minSkillLevel))
						{
							//Log.Message("no harvest - unskilled");
							continue;
						}

						// if it's valid for the 2n harvest to spawn
						Thing thing = ThingMaker.MakeThing(extH.harvestedThingDef);
						thing.stackCount = (int)extH.harvestYield;
						if (by.Faction != Faction.OfPlayer)
						{
							thing.SetForbidden(value: true);
						}
						GenPlace.TryPlaceThing(thing, by.Position, by.Map, ThingPlaceMode.Near);

					}
				}

			}
		}
    }
}