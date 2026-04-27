using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
	public class Biomes_PlantControl : DefModExtension
	{
		/// <summary>
		/// If set to false, the plant will grow all the time. The plant must have a thingClass of
		/// <thingClass>BMT.BiomesPlant</thingClass>.
		/// </summary>
		public bool needsRest = true;

		public List<string> terrainTags = new List<string>();

		/// <summary>
		/// Hours in which the plant will grow. The plant must have a thingClass of <thingClass>BMT.BiomesPlant</thingClass>.
		/// </summary>
		public FloatRange growingHours = new FloatRange(0.25F, 0.8F);

		/// <summary>
		/// Define a temperature range in which the plant will grow in optimal conditions. The plant must have a
		/// thingClass of <thingClass>BMT.BiomesPlant</thingClass>.
		/// </summary>
		//public FloatRange optimalTemperature = new FloatRange(6.0F, 42.0F);

		// The following attributes should be cleaned up in the future; mods and code are not using them anymore.
		public bool allowInCave = false;
		public bool allowInBuilding = false;
		public bool allowUnroofed = true;
		public bool wallGrower = false;
		public FloatRange lightRange = new FloatRange(0f, 1f);

		public override void ResolveReferences(Def parentDef)
		{
			base.ResolveReferences(parentDef);
			if (!terrainTags.NullOrEmpty() && parentDef is ThingDef thingDef)
			{
				if (thingDef.plant.wildTerrainTags == null)
				{
					thingDef.plant.wildTerrainTags = new();
				}
				foreach (string tag in terrainTags)
				{
					thingDef.plant.wildTerrainTags.Add(tag);
				}
				CheckFertility(thingDef);
			}
			HarmonyPatch_PlantsSpawnControl();
		}

		private void CheckFertility(ThingDef thingDef)
		{
			if (thingDef.plant.completelyIgnoreFertility)
			{
				return;
			}
			foreach (TerrainDef terrainDef in DefDatabase<TerrainDef>.AllDefsListForReading)
			{
				if (terrainDef.tags.NullOrEmpty() || !terrainDef.IsWater)
				{
					continue;
				}
				foreach (string tag in terrainDef.tags)
				{
					if (terrainTags.Contains(tag))
					{
						thingDef.plant.completelyIgnoreFertility = true;
						return;
					}
				}
			}
		}

		private static bool plantsSpawnPatched = false;
		// Please insert new patches here.Now this is a framework, let's make it flexible.
		private static void HarmonyPatch_PlantsSpawnControl()
		{
			if (plantsSpawnPatched)
			{
				return;
			}
			try
			{
				BiomesCore.Harmony.Patch(AccessTools.Method(typeof(PlantUtility), "CanEverPlantAt", [typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(bool), typeof(bool)]), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Patch_CanEverPlantAt))));
				//To-Do: Other Biomes_PlantControl patches
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply plants spawn control patch. Reason: " + arg.Message);
			}
			plantsSpawnPatched = true;
		}

	}
}