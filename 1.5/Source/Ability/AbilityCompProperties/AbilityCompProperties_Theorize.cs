namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class AbilityCompProperties_Theorize : CompProperties_AbilityEffect
	{
		public AbilityCompProperties_Theorize()
		{
			compClass = typeof(AbilityComp_Theorize);
		}

		public TheorizeAbilitySettings theorizeAbilitySettings;
	}
}
