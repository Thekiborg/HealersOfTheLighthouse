namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051

	public class AbilityCompProperties_Reloadable : AbilityCompProperties
	{
		public AbilityCompProperties_Reloadable()
		{
			compClass = typeof(AbilityComp_Reloadable);
		}

		public ThingDef ammoDef;
		public int baseReloadTicks;
		public int ammoCountPerCharge;
		public SoundDef soundReload;
	}
}
