using Verse.AI;
using Verse.Sound;

namespace HealersOfTheLighthouse
{
	public class JobDriver_ReloadConcealedLauncher : JobDriver
	{
		private AbilityComp_ConcealedLauncher CompCL => pawn.abilities?.GetAbility(HOTL_AbilityDefOfs.HOTL_ConcealedArmament_MarbleLauncher)?.CompOfType<AbilityComp_ConcealedLauncher>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(TargetA, job);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOn(() => CompCL is null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, TargetA.Cell);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A).FailOnDestroyedNullOrForbidden(TargetIndex.A);
			yield return Toils_General.Wait(120).WithProgressBarToilDelay(TargetIndex.A);
			yield return new Toil()
			{
				initAction = () =>
				{
					CompCL.Magazine[CompCL.MagazineSlotToReload].IsLoaded = true;
					pawn.carryTracker.CarriedThing?.SplitOff(1)?.Destroy();
					CompCL.Props.reloadSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
