namespace HealersOfTheLighthouse
{
	public class MapComponent_HOTL : MapComponent
	{
		private int wantedHemogenCount = 10;
		internal int WantedHemogenCount
		{
			get => wantedHemogenCount;
			set => wantedHemogenCount = Math.Max(value, 0);
		}


		public MapComponent_HOTL(Map map) : base(map) { }


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref wantedHemogenCount, "HOTL_MapComponent_WantedHemogenCount");
		}
	}
}
