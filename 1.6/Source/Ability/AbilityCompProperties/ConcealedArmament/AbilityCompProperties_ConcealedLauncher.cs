namespace HealersOfTheLighthouse
{
	public class AbilityCompProperties_ConcealedLauncher : CompProperties_AbilityEffect
	{
		public int ammoCapacity;
		public List<ThingDef> allowedAmmo;
		public bool displayGizmoWhileDrafted;
		public bool displayGizmoWhileUndrafted;
		public SoundDef reloadSound;
		public int burstSpreadRadiusSquared;
	}
}
