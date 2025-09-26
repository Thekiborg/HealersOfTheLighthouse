using System.Reflection;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class WorkGiver_ClearOperatingBed : WorkGiver_Scanner
	{
		private Building_Bed restingBed;
		private Pawn foundPawn;

		private readonly FieldInfo slabBedInfo = AccessTools.DeclaredField(typeof(RestUtility), "bedDefsBestToWorst_SlabBed_RestEffectiveness");
		private readonly FieldInfo bedInfo = AccessTools.DeclaredField(typeof(RestUtility), "bedDefsBestToWorst_RestEffectiveness");
		private List<ThingDef> bedDefsBestToWorst;
		private List<ThingDef> slabBedDefsBestToWorst;
		private FieldInfo SlabBedInfo
		{
			get
			{
				if (slabBedInfo is null) Log.Error("HealersOfTheLighthouse.WorkGiver_ClearOperatingBed.slabBedInfo is null. Check the new name in RestUtility.");
				return slabBedInfo;
			}
		}
		private FieldInfo BedInfo
		{
			get
			{
				if (bedInfo is null) Log.Error("HealersOfTheLighthouse.WorkGiver_ClearOperatingBed.bedInfo is null. Check the new name in RestUtility.");
				return bedInfo;
			}
		}
		private List<ThingDef> BedDefsBestToWorst
		{
			get
			{
				bedDefsBestToWorst ??= BedInfo.GetValue(null) as List<ThingDef>;
				return bedDefsBestToWorst;
			}
		}
		private List<ThingDef> SlabBedDefsBestToWorst
		{
			get
			{
				slabBedDefsBestToWorst ??= SlabBedInfo.GetValue(null) as List<ThingDef>;
				return slabBedDefsBestToWorst;
			}
		}
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(HOTL_ThingDefOfs.HOTL_OperatingBed);



		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is Building_Bed ORbed)
			{
				if (ORbed.GetCurOccupant(0) is Pawn pawnOnBed && !pawnOnBed.BillStack.AnyShouldDoNow)
				{
					foundPawn = pawnOnBed;
					FindRestingBedFor(pawn, pawnOnBed);
					return true;
				}
			}
			return false;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_ClearOperatingBed, t, restingBed, foundPawn);
			job.count = 1;
			return job;
		}


		private void FindRestingBedFor(Pawn traveler, Pawn sleeper)
		{
			List<ThingDef> bedList = PawnUsesSlabBeds(sleeper) ? SlabBedDefsBestToWorst : BedDefsBestToWorst;
			bedList.Remove(HOTL_ThingDefOfs.HOTL_OperatingBed);

			foreach (ThingDef bedDef in bedList)
			{
				if (sleeper.ownership?.OwnedBed is not null
					&& RestUtility.IsValidBedFor(sleeper.ownership.OwnedBed, sleeper, traveler, true)
					&& !HealthAIUtility.ShouldSeekMedicalRest(sleeper))
				{
					restingBed = sleeper.ownership.OwnedBed; // Should be the bed for sleeping
					Log.Message("No rest needed, bed = personal bed");
				}
				else
				{
					restingBed = GenClosest.ClosestThingReachable(
						sleeper.Position,
						traveler.Map,
						ThingRequest.ForDef(bedDef),
						PathEndMode.Touch,
						TraverseParms.For(traveler, MaxPathDanger(traveler)),
						validator: (Thing potentialBed) =>
						{
							return potentialBed is Building_Bed bed && RestUtility.IsValidBedFor(bed, sleeper, traveler, true);
						}
						) as Building_Bed;
					// Find hospital bed for injuries

					if (restingBed is null)
					{
						Log.Message("No free medical bed found");
					}
					else
					{
						Log.Message("Found hospital bed");
					}

					if (restingBed is not null) break;
				}
			}
		}


		private static bool PawnUsesSlabBeds(Pawn pawn)
		{
			if (pawn.Ideo != null)
			{
				foreach (Precept item in pawn.Ideo.PreceptsListForReading)
				{
					if (item.def.prefersSlabBed)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
