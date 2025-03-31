namespace HealersOfTheLighthouse
{
	public abstract class AbilityComp_ConcealedLightningArmament : CompAbilityEffect
	{
		const float RainRateThreshold = 0.4f;


		Pawn Caster => parent.pawn;
		protected int NumberOfJumps => IsRaining ? Props.wetJumpsCount : Props.dryJumpsCount;
		private bool IsRaining => Caster.Map.weatherManager.RainRate > RainRateThreshold;
		public new virtual AbilityCompProperties_ConcealedLightningProjectile Props => (AbilityCompProperties_ConcealedLightningProjectile)props;
	}
}
