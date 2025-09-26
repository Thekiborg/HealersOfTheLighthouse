using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_AssistSurgery : JobDriver
	{
		private JobDriver_DoBill doBillDriver;
		private Job doBillJob;


		private Pawn Surgeon => TargetA.Pawn;
		private static TargetIndex SurgeonIndex => TargetIndex.A;
		private IntVec3 CellAroundPatient => TargetB.Cell;
		private Thing Patient => TargetC.Pawn;
		private JobDriver_DoBill DoBillDriver
		{
			get
			{
				doBillDriver ??= Surgeon.jobs.curDriver as JobDriver_DoBill;
				return doBillDriver;
			}
		}

		private Job DoBillJob
		{
			get
			{
				doBillJob ??= Surgeon.jobs.curJob;
				return doBillJob;
			}
		}


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(CellAroundPatient, job);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			AddEndCondition(() =>
			{
				return DoBillDriver.workLeft <= 0 ? JobCondition.Succeeded : JobCondition.Ongoing;
			});
			AddEndCondition(() =>
			{
				return Surgeon.jobs.curDriver is not JobDriver_DoBill ? JobCondition.Succeeded : JobCondition.Ongoing;
			});


			yield return Toils_Goto.GotoCell(CellAroundPatient, PathEndMode.OnCell);
			Toil assistToil = ToilMaker.MakeToil("AssistToil");
			assistToil.defaultCompleteMode = ToilCompleteMode.Never;
			assistToil.WithEffect(() => DoBillJob.bill.recipe.effectWorking, SurgeonIndex);
			assistToil.PlaySustainerOrSound(() => DoBillJob.bill.recipe.soundWorking);
			assistToil.handlingFacing = true;
			assistToil.tickIntervalAction = delegate (int delta)
			{
				pawn.rotationTracker.FaceTarget(Patient);
				if (DoBillJob.RecipeDef.workSkill != null && DoBillJob.RecipeDef.UsesUnfinishedThing && pawn.skills != null)
				{
					pawn.skills.Learn(DoBillJob.RecipeDef.workSkill, 0.1f * DoBillJob.RecipeDef.workSkillLearnFactor * delta);
				}
				float extraSpeed = (DoBillJob.RecipeDef.workSpeedStat == null) ? 0.5f : (pawn.GetStatValue(DoBillJob.RecipeDef.workSpeedStat) / 2);
				if (DebugSettings.fastCrafting)
				{
					extraSpeed *= 30f;
				}
				DoBillDriver.workLeft -= extraSpeed * delta;
			};
			yield return assistToil;
		}
	}
}
