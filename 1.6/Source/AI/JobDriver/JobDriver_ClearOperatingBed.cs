using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_ClearOperatingBed : JobDriver
	{
		private Building_Bed ORbed => job.targetA.Thing as Building_Bed;
		private static TargetIndex ORbedTI => TargetIndex.A;
		private Thing NewBed => job.targetB.Thing;
		private static TargetIndex NewBedTI => TargetIndex.B;
		private Pawn RestingPawn => job.targetC.Pawn;
		private static TargetIndex RestingPawnTI => TargetIndex.C;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(RestingPawn, job);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil endToil = ToilMaker.MakeToil("EndJob");
			endToil.defaultCompleteMode = ToilCompleteMode.Instant;

			Toil noBedJump = ToilMaker.MakeToil("NoBedJump");
			noBedJump.defaultCompleteMode = ToilCompleteMode.Instant;

			yield return Toils_Goto.Goto(ORbedTI, PathEndMode.Touch);
			yield return Toils_Haul.StartCarryThing(RestingPawnTI);
			yield return Toils_Jump.JumpIf(noBedJump, () => NewBed is null);
			yield return Toils_Haul.CarryHauledThingToCell(NewBedTI, PathEndMode.Touch);
			yield return Toils_Bed.TuckIntoBed(NewBedTI, RestingPawnTI);
			yield return Toils_Jump.Jump(endToil);
			yield return noBedJump;
			yield return Toils_Haul.DropCarriedThing();
			yield return endToil;
		}
	}
}
