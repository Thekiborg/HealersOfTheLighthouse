﻿using System.Linq;
using Verse.AI;
using Verse.Sound;

namespace HealersOfTheLighthouse
{
	public class JobDriver_ReloadConcealedLauncher : JobDriver
	{
		private AbilityComp_ConcealedLauncher compCL;
		private AbilityComp_ConcealedLauncher CompCL
		{
			get
			{
				compCL ??= pawn.abilities?.abilities.Where(ab => ab.CompOfType<AbilityComp_ConcealedLauncher>()?.FindEmptySlot(out _) != false).FirstOrDefault()?.CompOfType<AbilityComp_ConcealedLauncher>();
				return compCL;
			}
		}


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
			Toil reloadToil = ToilMaker.MakeToil("ReloadConcealedLauncherSlot");
			reloadToil.AddPreInitAction(() =>
			{
				CompCL.FindEmptySlot(out int slotToReload);
				CompCL.Magazine[slotToReload].IsLoaded = true;
				pawn.carryTracker.CarriedThing?.SplitOff(1)?.Destroy();
				CompCL.Props.reloadSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
			});
			reloadToil.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return reloadToil;
		}
	}
}
