namespace HealersOfTheLighthouse
{
	public class AbilityCompProperties_Theorize : AbilityCompProperties_SDBD
	{
		public AbilityCompProperties_Theorize()
		{
			compClass = typeof(AbilityComp_Theorize);
		}

		public SDBDTheorizeSettings theorizeSettings;
	}
}
