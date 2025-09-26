namespace HealersOfTheLighthouse
{
	public class GameComponent_HOTL : GameComponent
	{
		internal static HashSet<Pawn> pawnsOnWheelchairs = [];
		internal static HashSet<Pawn> pawnsWithCrutches = [];
		private static int wantedHemogenCount = 10;
		internal static int WantedHemogenCount
		{
			get => wantedHemogenCount;
			set => wantedHemogenCount = Math.Max(value, 0);
		}


		public GameComponent_HOTL(Game game) { }


		public override void ExposeData()
		{
			Scribe_Values.Look(ref wantedHemogenCount, "HOTL_GameComp_WantedHemogenCount");
			Scribe_Collections.Look(ref pawnsOnWheelchairs, "HOTL_GameComp_PawnsOnWheelchairs", LookMode.Reference);
			Scribe_Collections.Look(ref pawnsWithCrutches, "HOTL_GameComp_PawnsWithCrutches", LookMode.Reference);
		}
	}
}
