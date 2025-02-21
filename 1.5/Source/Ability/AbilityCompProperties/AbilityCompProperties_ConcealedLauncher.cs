namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051, CA1002
	public class AbilityCompProperties_ConcealedLauncher : CompProperties_AbilityEffect
	{
		public AbilityCompProperties_ConcealedLauncher()
		{
			compClass = typeof(AbilityComp_ConcealedLauncher);
		}

		public int ammoCapacity;
		public List<ThingDef> allowedAmmo;
		public bool displayGizmoWhileDrafted;
		public bool displayGizmoWhileUndrafted;
		public bool disableGizmoWhileDrafted;
		public bool disableGizmoWhileUndrafted;
	}
}
