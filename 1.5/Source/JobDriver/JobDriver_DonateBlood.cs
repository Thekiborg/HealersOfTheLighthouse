using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_DonateBlood : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(TargetA, job);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.A);

			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);

			Toil waitToil = Toils_General.Wait(500).WithProgressBarToilDelay(TargetIndex.A);
			waitToil.tickAction = () =>
			{
				pawn.Rotation = TargetA.Thing.Rotation;
			};
			waitToil.FailOn(() =>
			{
				Hediff bloodLoss = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
				return bloodLoss is not null && bloodLoss.Severity < Workgiver_DonateBlood.MinBloodlossSeverity;
			});
			waitToil.handlingFacing = true;
			
			yield return waitToil;

			Toil toil = ToilMaker.MakeToil("SPAWNBLOOD");
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			toil.AddFinishAction(() =>
			{
				Hediff bloodloss = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
				bloodloss.Severity = 0.45f;
				pawn.health.AddHediff(bloodloss);

				Thing hemo = ThingMaker.MakeThing(ThingDefOf.HemogenPack);
				GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.HemogenPack), TargetA.Cell, Map, ThingPlaceMode.Near);
			});
			yield return toil;
		}
	}
}
