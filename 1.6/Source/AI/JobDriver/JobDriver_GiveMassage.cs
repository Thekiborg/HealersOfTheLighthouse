using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace HealersOfTheLighthouse
{
	public class JobDriver_GiveMassage : JobDriver
	{
		Pawn Bottom => TargetA.Pawn;
		static TargetIndex BottomIndex => TargetIndex.A;
		Building_MassageBed MassageBed => (Building_MassageBed)TargetB.Thing;
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
			this.FailOn(() => pawn.Drafted);

			Toil gotoBottle = Toils_Goto.GotoThing(OilBottleIndex, OilBottle.Position);
			gotoBottle.FailOnDespawnedNullOrForbidden(OilBottleIndex);
			gotoBottle.FailOnBurningImmobile(OilBottleIndex);
			gotoBottle.AddFinishAction(() => MassageBed.TopReached = true);


			Toil haulBottle = Toils_Haul.StartCarryThing(OilBottleIndex);


			Toil pathAroundBed = Toils_General.Do(() =>
			{
				int num = 0;
				IntVec3 cell;
				do
				{
					if (num > 100)
					{
						Log.Error($"{pawn} could not find standable cell adjacent to {MassageBed}");
						EndJobWith(JobCondition.Errored);
						return;
					}
					cell = MassageBed.RandomAdjacentCell8Way();
					num++;
				}
				while (!cell.Standable(Map) || !pawn.CanReach(cell, PathEndMode.OnCell, Danger.Deadly));

				pawn.pather.StartPath(cell, PathEndMode.OnCell);
			});
			pathAroundBed.FailOnDespawnedNullOrForbidden(MassageBedIndex);
			pathAroundBed.FailOnBurningImmobile(MassageBedIndex);
			pathAroundBed.defaultCompleteMode = ToilCompleteMode.PatherArrival;

			Toil waitNextToBed = Toils_General.Wait(240, BottomIndex);
			waitNextToBed.AddPreTickIntervalAction((int delta) =>
			{
				if (pawn.IsHashIntervalTick(80))
				{
					FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Heart);
				}
				JoyUtility.JoyTickCheckEnd(pawn, delta, JoyTickFullJoyAction.None, MassageSettings.CalculateJoyFactor(1f, pawn, Bottom, true), MassageBed);
			});

			Toil jumpWaitNextToBed = Toils_Jump.JumpIf(waitNextToBed, () =>
			{
				return MassageBed.BottomOnBed;
			});


			Toil waitForBottom = Toils_General.Wait(300);


			Toil jumpKeepWaiting = Toils_Jump.JumpIf(waitForBottom, () =>
			{
				return !MassageBed.BottomOnBed;
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
