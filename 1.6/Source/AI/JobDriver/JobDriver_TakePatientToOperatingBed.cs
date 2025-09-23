using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_TakePatientToOperatingBed : JobDriver
	{
		private Pawn Pawn => job.targetA.Pawn;
		private static TargetIndex PawnTI => TargetIndex.A;
		private Building_Bed ORbed => job.targetB.Thing as Building_Bed;
		private static TargetIndex ORbedTI => TargetIndex.B;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Pawn, job))
			{
				return pawn.Reserve(ORbed, job);
			}
			return false;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(PawnTI, PathEndMode.Touch);
			yield return Toils_Haul.StartCarryThing(PawnTI);
			yield return Toils_Haul.CarryHauledThingToCell(ORbedTI, PathEndMode.Touch);
			yield return Toils_Reserve.Release(ORbedTI);
			yield return Toils_Bed.TuckIntoBed(ORbedTI, PawnTI);
		}
	}
}
