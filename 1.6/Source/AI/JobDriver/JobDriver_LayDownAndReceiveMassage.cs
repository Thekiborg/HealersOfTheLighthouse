using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_LayDownAndReceiveMassage : JobDriver
	{
		private float joyFactor = -1f;
		private MassageSettings massageSettings;

		private Pawn Top => TargetA.Pawn;
		private static TargetIndex TopIndex => TargetIndex.A;
		private Building_MassageBed MassageBed => (Building_MassageBed)TargetB.Thing;
		private static TargetIndex MassageBedIndex => TargetIndex.B;
		private MassageSettings MassageSettings
		{
			get
			{
				massageSettings ??= job.def.joyKind.GetModExtension<ModExtension>().massageSettings;
				return massageSettings;
			}
		}

		public override Rot4 ForcedLayingRotation => MassageBed.Rotation.Opposite;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Top, job))
			{
				return pawn.Reserve(TargetB, job, job.def.joyMaxParticipants, 0, null, errorOnFailed);
			}
			return false;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => Top.CurJob.def != HOTL_JobDefOfs.HOTL_TopMassage);
			this.FailOnDespawnedNullOrForbidden(MassageBedIndex);
			this.FailOnAggroMentalState(TopIndex);
			AddFinishAction(delegate
			{
				Top.jobs.EndCurrentJob(JobCondition.Succeeded);
				MassageBed.ResetBed();
			});

			yield return Toils_General.Do(() =>
			{
				joyFactor = MassageSettings.CalculateJoyFactor(pawn, Top, false);
				if (joyFactor < 0f)
				{
					Log.Error("JobDriver_GiveMassage couldn't calculate the joy factor. Aborting.");
					EndJobWith(JobCondition.Errored);
				}
			});

			Toil gotoBed = Toils_Goto.GotoCell(BedUtility.GetFeetSlotPos(0, MassageBed.Position, MassageBed.Rotation, MassageBed.def.size), PathEndMode.OnCell);
			gotoBed.FailOnDespawnedNullOrForbidden(MassageBedIndex);
			gotoBed.FailOnBurningImmobile(MassageBedIndex);
			gotoBed.AddPreInitAction(() => pawn.mindState.lastJobTag = JobTag.SatisfyingNeeds);
			yield return gotoBed;

			Toil layDownToil = ToilMaker.MakeToil("LayOnMassageBed");
			layDownToil.socialMode = RandomSocialMode.Off;
			layDownToil.defaultDuration = job.def.joyDuration;
			layDownToil.defaultCompleteMode = ToilCompleteMode.Never;
			layDownToil.AddPreInitAction(() =>
			{
				pawn.jobs.posture = PawnPosture.LayingInBed;
				MassageBed.BottomOnBed = true;
			});
			layDownToil.AddPreTickIntervalAction((int delta) =>
			{
				if (ticksLeftThisToil <= 0 || pawn.needs.joy.CurLevel >= pawn.needs.joy.MaxLevel)
				{
					ReadyForNextToil();
				}
			});
			layDownToil.tickIntervalAction = (int delta) =>
			{
				pawn.GainComfortFromCellIfPossible(delta);

				if (MassageBed.TopReached)
				{
					if (pawn.IsHashIntervalTick(450))
					{
						MoteMaker.MakeSpeechBubble(pawn, TextureLibrary.heartIcon);
					}
					JoyUtility.JoyTickCheckEnd(pawn, delta, JoyTickFullJoyAction.None, joyFactor, MassageBed);
				}
			};
			yield return layDownToil;

			yield return Toils_General.DoAtomic(() =>
			{
				Thing OilBottle = Top.CurJob.GetTarget(TargetIndex.C).Thing;
				Thought_Memory oilThought = (Thought_Memory)ThoughtMaker.MakeThought(OilBottle.def.GetModExtension<ModExtension>().massageSettings.oilThought);
				Top.needs.mood.thoughts.memories.TryGainMemory(oilThought);
				pawn.needs.mood.thoughts.memories.TryGainMemory(oilThought);
				OilBottle.SplitOff(1).Destroy();
			});
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref joyFactor, "HOTL_ReceiveMassage_JoyFactor", -1f, true);
		}
	}
}
