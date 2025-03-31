namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class AbilityCompProperties_ConcealedShotgunArcThrower : AbilityCompProperties_ConcealedLightningProjectile
	{
		public AbilityCompProperties_ConcealedShotgunArcThrower()
		{
			compClass = typeof(AbilityComp_ConcealedShotgunArcThrower);
		}

		public int pelletCount;
	}
}
