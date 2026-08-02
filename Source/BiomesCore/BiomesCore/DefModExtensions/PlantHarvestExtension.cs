using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{

    public class PlantHarvestExtension : DefModExtension
    {
        public ThoughtDef memory = null;

		public List<ExtraHarvest> extraHarvests = new List<ExtraHarvest>();

		public class ExtraHarvest
		{
			public ThingDef harvestedThingDef;
			public float harvestYield = 1f;
			public float harvestChance = 1f;
			public float harvestMinGrowth = 0.65f;
			public int minSkillLevel = 0;
		}
	}

	[Obsolete("Please use PlantHarvestExtension instead.")]
	public class PlantHarvestMemoryExtension : PlantHarvestExtension
	{

	}

}