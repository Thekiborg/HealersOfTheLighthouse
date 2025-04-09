namespace HealersOfTheLighthouse
{
	public class Building_MassageBed : Building
	{
		private bool bottomOnBed;
		private bool topReached;


		public bool BottomOnBed { get; set; }
		public bool TopReached { get; set; }


		public void ResetBed()
		{
			BottomOnBed = false;
			TopReached = false;
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref bottomOnBed, "HOTL_MassageBed_BottomOnBed");
			Scribe_Values.Look(ref topReached, "HOTL_MassageBed_TopReached");
		}
	}
}
