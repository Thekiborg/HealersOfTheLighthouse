﻿namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class HediffCompProperties_StopBleedingInterval : HediffCompProperties
	{
		public HediffCompProperties_StopBleedingInterval()
		{
			compClass = typeof(HediffComp_StopBleedingInterval);
		}

		public FloatRange qualityRange;
		public int tickInterval;
	}
}
