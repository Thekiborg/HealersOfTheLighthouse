namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class AbilityCompProperties_LightningProjectile : CompProperties_AbilityEffect
	{
		public AbilityCompProperties_LightningProjectile()
		{
			compClass = typeof(AbilityComp_ArcThrower);
		}

		public int coneAngleDegs;
		public float coneRange;
	}
}
