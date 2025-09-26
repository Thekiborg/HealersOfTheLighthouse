using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_GiveMassage : JobDriver
	{
		private MassageSettings massageSettings;
		private Pawn Bottom => TargetA.Pawn;
		private static TargetIndex BottomIndex => TargetIndex.A;
		private Building_MassageBed MassageBed => (Building_MassageBed)TargetB.Thing;
		private static TargetIndex MassageBedIndex => TargetIndex.B;
		private Thing OilBottle => TargetC.Thing;
		private static TargetIndex OilBottleIndex => TargetIndex.C;
		private MassageSettings MassageSettings
		{
			get
			{
				massageSettings ??= job.def.joyKind.GetModExtension<ModExtension>().massageSettings;
				return massageSettings;
			}
		}


		private float joyFactor = -1f;


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

			yield return Toils_General.Do(() =>
			{
				joyFactor = MassageSettings.CalculateJoyFactor(pawn, Bottom, true);
				if (joyFactor < 0f)
				{
					Log.Error("JobDriver_GiveMassage couldn't calculate the joy factor. Aborting.");
					EndJobWith(JobCondition.Errored);
				}
			});

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
			pathAroundBed.socialMode = RandomSocialMode.Off;

			Toil waitNextToBed = Toils_General.Wait(240, BottomIndex);
			waitNextToBed.AddPreTickIntervalAction((int delta) =>
			{
				if (ticksLeftThisToil <= 0 || pawn.needs.joy.CurLevel >= pawn.needs.joy.MaxLevel)
				{
					ReadyForNextToil();
				}
			});
			waitNextToBed.AddPreTickIntervalAction((int delta) =>
			{
				if (pawn.IsHashIntervalTick(80))
				{
					FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Heart);
				}
				JoyUtility.JoyTickCheckEnd(pawn, delta, JoyTickFullJoyAction.None, joyFactor, MassageBed);
			});
			waitNextToBed.socialMode = RandomSocialMode.Off;

			Toil jumpWaitNextToBed = Toils_Jump.JumpIf(waitNextToBed, () =>
			{
				return MassageBed.BottomOnBed;
			});
			jumpWaitNextToBed.socialMode = RandomSocialMode.Off;


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


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref joyFactor, "HOTL_GiveMassage_JoyFactor", -1f, true);
		}
	}
}
