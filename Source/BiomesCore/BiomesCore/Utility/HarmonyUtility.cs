using BiomesCore.DefModExtensions;
using BiomesCore.Defs;
using BiomesCore.Patches;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BiomesCore
{
	// Please insert new patches here. Now this is a framework, let's make it flexible.
	public static class HarmonyUtility
	{

		public static bool IsVanillaDef(this Def def)
		{
			return def.modContentPack != null && def.modContentPack.IsOfficialMod;
		}

		// =====GenesBackground=====GenesBackground=====GenesBackground=====GenesBackground=====GenesBackground=====
		// =====GenesBackground=====GenesBackground=====GenesBackground=====GenesBackground=====GenesBackground=====
		// =====GenesBackground=====GenesBackground=====GenesBackground=====GenesBackground=====GenesBackground=====

		private static bool enableCustomBackground = true;
		public static bool Patch_GenesBackground(GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
		{
			if (enableCustomBackground && gene is BMT_GeneDef newGeneDef)
			{
				try
				{
					DrawGeneBasics(newGeneDef, geneRect, geneType, doBackground, clickable, overridden);
				}
				catch (Exception arg)
				{
					Log.Error("Failed render custom background for gene: " + gene.defName + ". Resetting BMT backgrounds to vanilla. Reason: " + arg);
					enableCustomBackground = false;
				}
				return false;
			}
			return true;
		}

		private static void DrawGeneBasics(BMT_GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
		{
			GUI.BeginGroup(geneRect);
			Rect rect = geneRect.AtZero();
			if (doBackground)
			{
				Widgets.DrawHighlight(rect);
				GUI.color = new(1f, 1f, 1f, 0.05f);
				Widgets.DrawBox(rect);
				GUI.color = Color.white;
			}
			float num = rect.width - Text.LineHeight;
			Rect rect2 = new(geneRect.width / 2f - num / 2f, 0f, num, num);
			Color iconColor = gene.IconColor;
			if (overridden)
			{
				iconColor.a = 0.75f;
				GUI.color = ColoredText.SubtleGrayColor;
			}
			CachedTexture cachedTexture = gene.BackgroundTexture(gene, geneType);
			GUI.DrawTexture(rect2, cachedTexture.Texture);
			Widgets.DefIcon(rect2, gene, null, 0.9f, null, drawPlaceholder: false, iconColor);
			Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(gene.LabelCap, rect.width);
			Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			if (overridden)
			{
				GUI.color = ColoredText.SubtleGrayColor;
			}
			if (doBackground && num2 < (Text.LineHeight - 2f) * 2f)
			{
				rect3.y -= 3f;
			}
			Widgets.Label(rect3, gene.LabelCap);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			if (clickable)
			{
				if (Widgets.ButtonInvisible(rect))
				{
					Find.WindowStack.Add(new Dialog_InfoCard(gene));
				}
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
			}
			GUI.EndGroup();
		}

		// =====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====
		// =====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====
		// =====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====PlantsSpawn=====

		public static void Patch_CanEverPlantAt(ref bool __result, ThingDef plantDef, IntVec3 c, Map map)
		{
			if (plantDef.IsVanillaDef())
			{
				return;
			}
			string phase = "initial";
			try
			{
				phase = "get plant ModExtension";
				Biomes_PlantControl plantExt = plantDef.GetModExtension<Biomes_PlantControl>();
				// Ignore defs without extension
				if (plantExt == null || plantExt.terrainTags.NullOrEmpty())
				{
					return;
				}
				phase = "check grid";
				List<Thing> list = map.thingGrid.ThingsListAt(c);
				foreach (Thing thing in list) //governs plant that grow on buildings, such as planters or hydroponics systems. These should bypass our other checks.
				{
					if (thing?.def.building != null)
					{
						return;
					}
				}
				phase = "get terrain";
				TerrainDef terrain = map.terrainGrid.TerrainAt(c);
				Biomes_PlantControl terrainExt = terrain.GetModExtension<Biomes_PlantControl>();
				if (terrainExt != null)
				{
					//Log.Error(plantDef.defName + " " + terrain.defName);
					phase = "check vanilla fertility and tags";
					if (terrain.fertility <= 0f || terrain.IsWater)
					{
						plantDef.plant.completelyIgnoreFertility = true;
					}
					phase = "check BMT tags";
					foreach (string tag in terrainExt.terrainTags)
					{
						if (plantExt.terrainTags.Contains(tag))
						{
							//__result = true;
							return;
						}
					}
					__result = false;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Error in CanEverPlantAt patch. On phase: " + phase + ". Reason: " + arg.Message);
			}
		}

		// =====MISC=====MISC=====MISC=====MISC=====MISC=====
		// =====MISC=====MISC=====MISC=====MISC=====MISC=====
		// =====MISC=====MISC=====MISC=====MISC=====MISC=====

		public static void ModCompatibility()
		{
			foreach (ModContentPack modContentPack in LoadedModManager.RunningMods)
			{
				try
				{
					if (modContentPack.PackageId == "vanillaexpanded.vanillatradingexpanded")
					{
						VTE_Compt();
						continue;
					}
					if (modContentPack.PackageId == "biomesteam.biomescore")
					{
						BCore_Initial();
						continue;
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed patch in ModCompatibility() for mod: " + modContentPack.ModMetaData.Name + ". Reason: " + arg.Message);
				}
			}
		}

		private static void VTE_Compt()
		{
			var vteGenerateCarriers =
				AccessTools.Method("VanillaTradingExpanded.IncidentWorker_CaravanArriveForItems:GenerateCarriers");
			var transpileCarriers =
				new HarmonyMethod(AccessTools.Method(typeof(PawnGroupKindWorker_Trader_GenerateCarriers),
					nameof(PawnGroupKindWorker_Trader_GenerateCarriers.Transpiler)));
			BiomesCore.Harmony.Patch(vteGenerateCarriers, transpiler: transpileCarriers);
		}

		private static void BCore_Initial()
		{
			// To-Do: Info stat patch
		}

		// Animals control
		public static bool Patch_IsAcceptablePreyFor(ref bool __result, ref Pawn predator, ref Pawn prey)
		{
			Biomes_AnimalControl animalControl = predator.def?.GetModExtension<Biomes_AnimalControl>();
			if (animalControl != null)
			{
				if (animalControl.canHuntOnlyOnDefs.NullOrEmpty())
				{
					return true;
				}
				if (animalControl.canHuntOnlyOnDefs.Contains(prey.def))
				{
					return true;
				}
				__result = false;
				return false;
			}
			return true;
		}

		public static IEnumerable<PawnKindDef> Patch_ComplexThreatWorker_SleepingInsects(IEnumerable<PawnKindDef> values)
		{
			foreach (var value in values)
			{
				var extension = value.race.GetModExtension<Biomes_AnimalControl>();

				if (extension == null || extension.isInsectoid)
				{
					yield return value;
				}
			}
		}

		public static void PollutionUtility_StimulatedByPollution_Patch(ref bool __result, Pawn pawn)
		{
			if (__result)
			{
				var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
				__result = extension == null || extension.isInsectoid;
			}
		}

	}

}
