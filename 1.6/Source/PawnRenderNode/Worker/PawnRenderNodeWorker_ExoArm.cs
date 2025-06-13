namespace HealersOfTheLighthouse
{
	public class PawnRenderNodeWorker_ExoArm : PawnRenderNodeWorker
	{
		public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
		{
			Hediff exoArmHediff = parms.pawn.health.hediffSet.GetFirstHediffOfDef(HOTL_HediffDefOfs.HOTL_Installed_ExoArm);

			if (exoArmHediff.Severity >= (node?.Props as PawnRenderNodeProperties_ExoArm).severityToShow)
			{
				return base.CanDrawNow(node, parms);
			}
			else
			{
				return false;
			}
		}
	}
}
