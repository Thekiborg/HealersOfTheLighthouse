namespace HealersOfTheLighthouse
{
	public class AbilityCompProperties_ConcealedLightningProjectile : CompProperties_AbilityEffect
	{
		public AbilityCompProperties_ConcealedLightningProjectile()
		{
			compClass = typeof(AbilityComp_ConcealedArcThrower);
		}

		public int arcConeAngleDegs;
		public float arcConeRange;
		public int dryJumpsCount;
		public int wetJumpsCount;
		public int stunDurationTicks;
	}
}
