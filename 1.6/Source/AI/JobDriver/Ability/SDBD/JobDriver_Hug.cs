using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_Hug : JobDriver
	{
		public Pawn HugTarget => TargetA.Pawn;
		private const int HugDuration = 200;
		private AbilityComp_Hug compHug;


		private AbilityComp_Hug CompHug
		{
			get
			{
				compHug ??= pawn.abilities.GetAbility(HOTL_AbilityDefOfs.HOTL_SDBD_AbilityHug).CompOfType<AbilityComp_Hug>();
				return compHug;
			}
		}


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(HugTarget, job);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => !HugTarget.InMentalState);

			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

			Toil hug = Toils_General.Wait(HugDuration, TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A);
			hug.initAction = delegate
			{
				pawn.pather.StopDead();
				PawnUtility.ForceWait(HugTarget, HugDuration, pawn, true);
			};
			yield return hug;

			yield return Toils_General.Do(() =>
			{
				if (SDBDUtilities.TryToStopMentalState(HugTarget))
				{
					CompHug.AbilityUsed();
					pawn.interactions.TryInteractWith(HugTarget, CompHug.Props.interactionDef);
					HugTarget.MentalState.RecoverFromState();
					HugTarget.needs?.mood?.thoughts?.memories?.TryGainMemory(HOTL_ThoughtDefOfs.HOTL_HuggedColonist, pawn);
				}
			});
		}
	}
}
