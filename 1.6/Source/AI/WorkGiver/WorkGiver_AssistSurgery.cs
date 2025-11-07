using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class WorkGiver_AssistSurgery : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);
		private Pawn patient;
		private IntVec3 cellAroundPatient;


		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is Pawn surgeon)
			{
				if (surgeon.jobs.curDriver is JobDriver_DoBill doBill)
				{
					if (doBill.CurToilString != "DoRecipeWork") return false;

					patient = doBill.job.targetA.Pawn;
					if (patient?.CurrentBed() is not null)
					{
						cellAroundPatient = FindFreeCellAroundPatient(pawn, patient);
						return true;
					}
				}
			}
			return false;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_AssistSurgery, t, cellAroundPatient, patient);
			return job;
		}


		private IntVec3 FindFreeCellAroundPatient(Pawn helper, Pawn patient)
		{
			int num = 0;
			IntVec3 cell;
			do
			{
				if (num > 100)
				{
					return IntVec3.Invalid;
				}
				cell = patient.CurrentBed().RandomAdjacentCell8Way();
				num++;
			}
			while (cell.GetThingList(patient.Map).Any(t => t is Pawn)
					|| !cell.Standable(patient.Map)
					|| !helper.CanReach(cell, PathEndMode.OnCell, MaxPathDanger(helper))
					|| !helper.CanReserve(cell));
			return cell;
		}
	}
}
