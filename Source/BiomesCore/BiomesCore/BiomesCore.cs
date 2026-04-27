using HarmonyLib;
using System;
using BiomesCore.ModSettings;
using BiomesCore.Patches;
using UnityEngine;
using Verse;

namespace BiomesCore
{
	public class BiomesCore : Mod
	{
		public const string Id = "rimworld.biomes.core";
		public const string Name = "Biomes! Core";
		private static readonly Version Version = typeof(BiomesCore).Assembly.GetName().Version;

		private static Harmony harmonyInstance;
		public static Harmony Harmony
		{
			get
			{
				if (harmonyInstance == null)
				{
					harmonyInstance = new Harmony(Id);
				}
				return harmonyInstance;
			}
		}

		public BiomesCore(ModContentPack content) : base(content)
		{
			// Regular harmony patches.
			//harmonyInstance = new Harmony(Id);
			Harmony.PatchAll();
			// Conditional Harmony patches. Mostly intended for mod compatibility.
			HarmonyUtility.ModCompatibility();

			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			Log("Initialized");
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return SettingsWindow.SettingsCategory();
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			SettingsWindow.DoWindowContents(inRect);
			base.DoSettingsWindowContents(inRect);
		}

		public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
		public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

		private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
	}
}