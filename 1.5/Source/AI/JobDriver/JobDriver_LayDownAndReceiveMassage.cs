using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_LayDownAndReceiveMassage : JobDriver
	{
		Pawn Top => TargetA.Pawn;
		static TargetIndex TopIndex => TargetIndex.A;
		Thing MassageBed => TargetB.Thing;
		static TargetIndex MassageBedIndex => TargetIndex.B;
		public bool onBed;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Top, job))
			{
				return pawn.Reserve(MassageBed, job, job.def.joyMaxParticipants, 0, null, errorOnFailed);
			}
			return false;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => Top.CurJob.def != HOTL_JobDefOfs.HOTL_TopMassage);
			this.FailOnAggroMentalState(TopIndex);
			AddFinishAction(delegate
			{
				Top.jobs.EndCurrentJob(JobCondition.Errored);
			});

			Toil gotoBed = Toils_Bed.GotoBed(MassageBedIndex);
			gotoBed.FailOnDespawnedNullOrForbidden(MassageBedIndex);
			gotoBed.FailOnBurningImmobile(MassageBedIndex);
			yield return gotoBed;

			Toil layDownToil = Toils_LayDown.LayDown(MassageBedIndex, true, false, false, true, PawnPosture.LayingInBed, false);
			layDownToil.handlingFacing = true;
			layDownToil.socialMode = RandomSocialMode.Off;
			layDownToil.defaultDuration = job.def.joyDuration;
			layDownToil.defaultCompleteMode = ToilCompleteMode.Delay;
			layDownToil.AddPreInitAction(() => onBed = true);
			layDownToil.tickAction = () =>
			{
				// Check for top's arrival to bed signal
				if (true && pawn.IsHashIntervalTick(450))
				{
					Log.Message(ticksLeftThisToil);
					MoteMaker.MakeSpeechBubble(pawn, TextureLibrary.heartIcon);
				}
			};
			yield return layDownToil;

			yield return Toils_General.Do(() => pawn.Kill(new DamageInfo(DamageDefOf.Bullet, 20000)));

			/*Toil waitToil = Toils_General.Wait(job.def.joyDuration);
			waitToil.handlingFacing = true;
			waitToil.socialMode = RandomSocialMode.Off;
			waitToil.tickAction = () =>
			{
				if (pawn.IsHashIntervalTick(450))
				{
					Log.Error("a");
					MoteMaker.MakeSpeechBubble(pawn, TextureLibrary.lovinHeartIcon);
				}
			};
			yield return waitToil;*/
		}
	}
}
