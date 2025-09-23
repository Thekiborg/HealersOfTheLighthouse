using System.Linq;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class Workgiver_TakePatientToOperatingBed : WorkGiver_Scanner
	{
		private Building_Bed foundBed;
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);


		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is Pawn foundPawn)
			{
				if (pawn == foundPawn) return false; // Would this even happen?

				if (foundPawn.BillStack.AnyShouldDoNow && foundPawn.InBed() && pawn.CanReserve(foundPawn))
				{
					if (foundPawn.CurrentBed().def == HOTL_ThingDefOfs.HOTL_OperatingBed)
					{
						return false; // Already at the OR!
					}
					else
					{
						foundBed = GenClosest.ClosestThingReachable(
							foundPawn.Position,
							pawn.Map,
							ThingRequest.ForDef(HOTL_ThingDefOfs.HOTL_OperatingBed),
							PathEndMode.Touch,
							TraverseParms.For(pawn, MaxPathDanger(pawn)),
							validator: (Thing potentialBed) =>
								{
									return potentialBed is Building_Bed bed && bed.AnyUnoccupiedSleepingSlot && foundPawn.CanReserve(bed);
								}
							) as Building_Bed;

						return foundBed is not null;
					}
				}
			}
			return false;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_MovePatientToOperatingBed, t, foundBed);
			job.count = 1;
			return job;
		}
	}
}
