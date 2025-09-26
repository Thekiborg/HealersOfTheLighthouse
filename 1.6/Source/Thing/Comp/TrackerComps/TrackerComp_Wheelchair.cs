namespace HealersOfTheLighthouse
{
	public class TrackerComp_Wheelchair : ThingComp
	{
		public override void Notify_Equipped(Pawn pawn)
		{
			base.Notify_Equipped(pawn);
			GameComponent_HOTL.pawnsOnWheelchairs.Add(pawn);
			pawn.health.CheckForStateChange(null, null);
		}


		public override void Notify_Unequipped(Pawn pawn)
		{
			base.Notify_Unequipped(pawn);
			GameComponent_HOTL.pawnsOnWheelchairs.Remove(pawn);
			pawn.health.CheckForStateChange(null, null);
		}
	}
}
