namespace HealersOfTheLighthouse
{
	public class Comp_BloodDonationChair : ThingComp
	{
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			yield return new DesiredHemogenGizmo(parent.Map.GetComponent<MapComponent_HOTL>());
		}
	}
}
