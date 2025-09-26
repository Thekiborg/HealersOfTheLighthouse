namespace HealersOfTheLighthouse
{
	public class TrackerComp_Crutch : ThingComp
	{
		public override void Notify_Equipped(Pawn pawn)
		{
			base.Notify_Equipped(pawn);
			GameComponent_HOTL.pawnsWithCrutches.Add(pawn);
			pawn.health.CheckForStateChange(null, null);

		}


		public override void Notify_Unequipped(Pawn pawn)
		{
			base.Notify_Unequipped(pawn);
			GameComponent_HOTL.pawnsWithCrutches.Remove(pawn);
			pawn.health.CheckForStateChange(null, null);
		}
	}
}
