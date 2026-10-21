using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    public class CompProperties_BottomFeeder : CompProperties
    {
        public int tickInterval = 250;
        public string consumingFoodReportString = "Eating food";
		public bool excludePredators = false;
        public EffecterDef effecterDef;
        public string feedingTerrainTag;
        public bool shouldGlow = false; // For this to work, you need to give them the comp "CompProperties_DefaultOffGlower", too.
        public bool eatWhileMoving = false;
        public float foodGainPerTick = 0.002f;
        public CompProperties_BottomFeeder() => this.compClass = typeof(CompBottomFeeder);
    }
    public class CompBottomFeeder : ThingComp
    {
        public CompProperties_BottomFeeder Props => (CompProperties_BottomFeeder)this.props;

		public override void CompTickInterval(int delta)
		{
			if (!parent.IsHashIntervalTick(6000, delta))
			{
				return;
			}
			if (parent is not Pawn pawn)
			{
				return;
			}
			Eater(pawn);
		}

		private bool Eater(Pawn pawn)
		{
			if (Props.excludePredators && pawn.RaceProps.predator)
			{
				return false;
			}
			if (pawn.Faction == Faction.OfPlayerSilentFail)
			{
				if ((pawn.needs?.food) == null || pawn.needs.food.CurLevelPercentage >= 0.3f)
				{
					return false;
				}
				var targetCell = IntVec3.Invalid;
				bool desperate = pawn.needs.food.Starving;
				if (Props.eatWhileMoving)
				{
					pawn.Map.floodFiller.FloodFill(pawn.Position,
						c => c.GetTerrain(pawn.Map).HasTag(Props.feedingTerrainTag),
						c =>
						{
							if (pawn.Position.InHorDistOf(c, 10)) return false;
							if (!pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly)) return false;
							if (c.IsForbidden(pawn) && !desperate) return false;
							targetCell = c;
							return true;
						}
					);
				}
				else
				{
					pawn.Map.floodFiller.FloodFill(pawn.Position, c => true, c =>
					{
						if (!c.GetTerrain(pawn.Map).HasTag(Props.feedingTerrainTag)) return false;
						if (!pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly)) return false;
						if (c.IsForbidden(pawn) && !desperate) return false;
						targetCell = c;
						return true;
					});
				}
				if (targetCell.IsValid)
				{
					pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(BiomesCoreDefOf.BC_BottomFeeder, targetCell), JobTag.SatisfyingNeeds);
				}
			}
			else
			{
				if (pawn.needs?.food != null)
				{
					pawn.needs.food.CurLevelPercentage = 1f;
				}
			}
			return true;
		}
	}
}
