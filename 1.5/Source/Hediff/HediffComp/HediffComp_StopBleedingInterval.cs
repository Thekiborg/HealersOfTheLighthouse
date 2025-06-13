namespace HealersOfTheLighthouse
{
	public class HediffComp_StopBleedingInterval : HediffComp
	{
		public HediffCompProperties_StopBleedingInterval Props => (HediffCompProperties_StopBleedingInterval)props;


		public override void CompPostTick(ref float severityAdjustment)
		{
			if (Pawn.IsHashIntervalTick(Props.tickInterval))
			{
				foreach (Hediff hediff in Pawn.health.hediffSet.hediffs)
				{
					if ((hediff is Hediff_Injury || hediff is Hediff_MissingPart) && hediff.TendableNow() && hediff.Bleeding)
					{
						hediff.Tended(Props.qualityRange.RandomInRange, Props.qualityRange.TrueMax, 1);
						return;
					}
				}
			}
		}
	}
}
