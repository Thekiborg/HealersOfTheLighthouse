namespace HealersOfTheLighthouse
{
	public class GameComponent_HOTL : GameComponent
	{
		static int wantedHemogenCount = 10;
		internal static int WantedHemogenCount
		{
			get => wantedHemogenCount;
			set
			{
				wantedHemogenCount = Math.Max(value, 0);
			}
		}

		public GameComponent_HOTL(Game game) { }


		public override void ExposeData()
		{
			Scribe_Values.Look(ref wantedHemogenCount, "HOTL_GameComp_WantedHemogenCount");
		}
	}
}
