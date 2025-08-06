using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_ReloadAbility : JobDriver
	{
		private AbilityComp_Reloadable Reloadable => job.ability.CompOfType<AbilityComp_Reloadable>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.B), job);
			return true;
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => Reloadable is null);
			this.FailOn(() => !Reloadable.NeedsReload(allowForceReload: true));
			this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
			Toil getNextIngredient = Toils_General.Label();
			yield return getNextIngredient;
			foreach (Toil item in ReloadAsMuchAsPossible(Reloadable))
			{
				yield return item;
			}
			yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
			yield return Toils_Jump.JumpIf(getNextIngredient, () => !job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
			foreach (Toil item2 in ReloadAsMuchAsPossible(Reloadable))
			{
				yield return item2;
			}
			Toil toil = ToilMaker.MakeToil("MakeNewToils");
			toil.initAction = delegate
			{
				Thing carriedThing = pawn.carryTracker.CarriedThing;
				if (carriedThing != null && !carriedThing.Destroyed)
				{
					pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out var _);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return toil;
		}

		private IEnumerable<Toil> ReloadAsMuchAsPossible(AbilityComp_Reloadable reloadable)
		{
			Toil done = Toils_General.Label();
			yield return Toils_Jump.JumpIf(done, () => pawn.carryTracker.CarriedThing == null || pawn.carryTracker.CarriedThing.stackCount < reloadable.MinAmmoNeeded(allowForcedReload: true));
			yield return Toils_General.Wait(reloadable.BaseReloadTicks).WithProgressBarToilDelay(TargetIndex.A);
			Toil toil = ToilMaker.MakeToil("ReloadAsMuchAsPossible");
			toil.initAction = delegate
			{
				Thing carriedThing = pawn.carryTracker.CarriedThing;
				reloadable.ReloadFrom(carriedThing);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return toil;
			yield return done;
		}
	}
}
