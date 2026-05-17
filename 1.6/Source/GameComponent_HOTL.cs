namespace HealersOfTheLighthouse
{
	public class GameComponent_HOTL : GameComponent
	{
		internal static HashSet<Pawn> pawnsOnWheelchairs = [];
		internal static HashSet<Pawn> pawnsWithCrutches = [];


		public GameComponent_HOTL(Game game) { }


		public override void ExposeData()
		{
			Scribe_Collections.Look(ref pawnsOnWheelchairs, "HOTL_GameComp_PawnsOnWheelchairs", LookMode.Reference);
			Scribe_Collections.Look(ref pawnsWithCrutches, "HOTL_GameComp_PawnsWithCrutches", LookMode.Reference);
		}
	}
}
