using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Verse;

namespace BiomesCore.DefModExtensions
{
    public class Biomes_AnimalControl : DefModExtension
    {
        public List<string> biomesAlternateGraphics = new List<string>();
        
        
        /// <summary>
        /// The creature will eat the custom things described below even when it is fed.
        /// </summary>
        public bool eatWhenFed;

        public bool isBloodDrinkingAnimal;
        public bool isCustomThingEater;
        public bool isBottomFeeder;

        public List<ThingDef> canHuntOnlyOnDefs;

        /// <summary>
        /// Unless set to true, the animal will not be considered insectoid even if they have insectoid fleshtype or are
        /// an insect.
        /// * The animal will not spawn as an enemy in ancient complexes.
        /// * The animal will not be stimulated by pollution.
        /// * The animal can be chosen as a venerated animal.
        /// </summary>
        public bool isInsectoid;

		public override void ResolveReferences(Def parentDef)
		{
			base.ResolveReferences(parentDef);
			HarmonyPatch_AnimalsControl();
		}

		private static bool animalsControlPatched = false;
		// Please insert new patches here. Now this is a framework, let's make it flexible.
		private static void HarmonyPatch_AnimalsControl()
		{
			if (animalsControlPatched)
			{
				return;
			}
			string phase = "";
			try
			{
				phase = "0";
				BiomesCore.Harmony.Patch(AccessTools.Method(typeof(FoodUtility), "IsAcceptablePreyFor"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Patch_IsAcceptablePreyFor))));
				phase = "1";
				BiomesCore.Harmony.Patch(AccessTools.Method(typeof(ComplexThreatWorker_SleepingInsects), "GetPawnKindsForPoints"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Patch_ComplexThreatWorker_SleepingInsects))));
				phase = "2";
				BiomesCore.Harmony.Patch(AccessTools.Method(typeof(PollutionUtility), "StimulatedByPollution"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.PollutionUtility_StimulatedByPollution_Patch))));
				//To-Do: Other Biomes_AnimalControl patches
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply animals control patch. " + "On phase: " + phase + ". Reason: " + arg.Message);
			}
			animalsControlPatched = true;
		}
	}
}
