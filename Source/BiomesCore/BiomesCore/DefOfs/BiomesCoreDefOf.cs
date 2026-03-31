using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
	public class MayRequireBiomesCoreAttribute : MayRequireAttribute
	{
		public MayRequireBiomesCoreAttribute()
			: base("biomesteam.biomescore")
		{
		}
	}

	[DefOf]
	public static class BiomesCoreDefOf
	{
		[MayRequireBiomesCore]
		public static ThingDef BMT_LavaGenerator;

		[MayRequireBiomesCore]
		public static ConceptDef BMT_HungeringAnimalsConcept;

		[MayRequireBiomesCore]
		public static DutyDef BMT_WanderAroundPoint;

		[MayRequireBiomesCore]
		public static HediffDef BMT_HungeringHediff;

		[MayRequireBiomesCore]
		public static JobDef BC_BloodDrinking;
		[MayRequireBiomesCore]
		public static JobDef BC_BottomFeeder;
		[MayRequireBiomesCore]
		public static JobDef BMT_DevourHungering;
		[MayRequireBiomesCore]
		public static JobDef BC_EatCustomThing;
		[MayRequireBiomesCore]
		public static JobDef BC_HarvestAnimalProduct;
		[MayRequireBiomesCore]
		public static JobDef BC_HermaphroditicMate;

		[MayRequireBiomesCore]
		public static MentalStateDef BMT_Hungering;

		[MayRequireBiotech]
		public static EffecterDef CellPollution;

		[MayRequireBiomesCore]
		public static RoofDef BMT_RockRoofStable;

		[MayRequireBiomesCore]
		public static TerrainAffordanceDef BMT_TerrainAffordance_Lava;

		static BiomesCoreDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
		}
	}
}