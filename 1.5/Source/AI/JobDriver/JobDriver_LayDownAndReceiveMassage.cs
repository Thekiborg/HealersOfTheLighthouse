using System.Linq;
using System.Runtime.InteropServices;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_LayDownAndReceiveMassage : JobDriver
	{
		public override Rot4 ForcedLayingRotation => MassageBed.Rotation.Opposite;

		Pawn Top => TargetA.Pawn;
		static TargetIndex TopIndex => TargetIndex.A;
		Building_MassageBed MassageBed => (Building_MassageBed)TargetB.Thing;
		static TargetIndex MassageBedIndex => TargetIndex.B;


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
			layDownToil.AddPreTickAction(() =>
			{
				if (ticksLeftThisToil <= 0 || pawn.needs.joy.CurLevel >= pawn.needs.joy.MaxLevel)
				{
					ReadyForNextToil();
				}
			});
			layDownToil.tickAction = () =>
			{
				pawn.GainComfortFromCellIfPossible();

				if (MassageBed.TopReached)
				{
					if (pawn.IsHashIntervalTick(450))
					{
						MoteMaker.MakeSpeechBubble(pawn, TextureLibrary.heartIcon);
					}
					JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None, MassageSettings.CalculateJoyFactor(1f, pawn, Top, false), MassageBed);
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
	}
}
