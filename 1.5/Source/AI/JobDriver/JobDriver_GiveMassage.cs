using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_GiveMassage : JobDriver
	{
		Pawn Bottom => TargetA.Pawn;
		static TargetIndex BottomIndex => TargetIndex.A;
		Thing MassageBed => TargetB.Thing;
		static TargetIndex MassageBedIndex => TargetIndex.B;
		Thing OilBottle => TargetC.Thing;
		static TargetIndex OilBottleIndex => TargetIndex.C;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Bottom, job) && pawn.Reserve(MassageBed, job, job.def.joyMaxParticipants, 0, null, errorOnFailed))
			{
				return pawn.Reserve(OilBottle, job);
			}
			return false;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnAggroMentalState(BottomIndex);

			Toil gotoBottle = Toils_Goto.GotoThing(OilBottleIndex, OilBottle.Position);
			gotoBottle.FailOnDespawnedNullOrForbidden(OilBottleIndex);
			gotoBottle.FailOnBurningImmobile(OilBottleIndex);


			Toil haulBottle = Toils_Haul.StartCarryThing(OilBottleIndex);


			Toil pathAroundBed = Toils_General.Do(() =>
			{
				GenRadial.RadialCellsAround(MassageBed.Position, 1.9f, false).TryRandomElement(cell => cell.GetFirstBuilding(Map) is null, out var cell);
				pawn.pather.StartPath(cell, PathEndMode.OnCell);
			});
			pathAroundBed.FailOnDespawnedNullOrForbidden(MassageBedIndex);
			pathAroundBed.FailOnBurningImmobile(MassageBedIndex);
			pathAroundBed.defaultCompleteMode = ToilCompleteMode.PatherArrival;


			// Jump toil into waitNextToBed
			// Wait toil here just to wait for the bottom to come
			// Jump toil into the former wait toil until the bottom in bed signal

			Toil waitNextToBed = Toils_General.Wait(240, BottomIndex);
			waitNextToBed.AddPreTickAction(() =>
			{
				if (pawn.IsHashIntervalTick(80))
				{
					FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Heart);
				}
			});

			Toil jumpWaitNextToBed = Toils_Jump.JumpIf(waitNextToBed, () =>
			{
				return true; //((JobDriver_LayDownAndReceiveMassage)Bottom.CurJob.GetCachedDriver(Bottom)).onBed;
			});


			Toil waitForBottom = Toils_General.Wait(300);


			Toil jumpKeepWaiting = Toils_Jump.JumpIf(waitForBottom, () =>
			{
				return !((JobDriver_LayDownAndReceiveMassage)Bottom.CurJob.GetCachedDriver(Bottom)).onBed;
			});


			yield return gotoBottle;
			yield return haulBottle;
			yield return Toils_Goto.GotoThing(MassageBedIndex, PathEndMode.ClosestTouch);
			yield return jumpWaitNextToBed;
			yield return waitForBottom;
			yield return jumpKeepWaiting;
			yield return waitNextToBed;
			yield return pathAroundBed;
			yield return Toils_Jump.Jump(waitNextToBed);
		}
	}
}
