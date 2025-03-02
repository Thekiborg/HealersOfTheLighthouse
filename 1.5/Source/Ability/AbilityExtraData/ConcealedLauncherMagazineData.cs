﻿namespace HealersOfTheLighthouse
{
	public class ConcealedLauncherMagazineData : IExposable
	{
		private ThingDef thingDef;
		private bool isLoaded;


		public ThingDef ThingDef
		{
			get => thingDef;
			set
			{
				thingDef = value;
				if (thingDef is null)
				{
					isLoaded = false;
				}
			}
		}

		public bool IsLoaded
		{
			get => isLoaded;
			set
			{
				if (ThingDef is not null)
				{
					isLoaded = value;
				}
			}
		}


		public ConcealedLauncherMagazineData() { }
		public ConcealedLauncherMagazineData(ThingDef thingDef, bool isLoaded)
		{
			this.thingDef = thingDef;
			this.isLoaded = isLoaded;
		}


		public virtual void ExposeData()
		{
			Scribe_Defs.Look(ref thingDef, "HOTL_MagazineData_thingDef");
			Scribe_Values.Look(ref isLoaded, "HOTL_MagazineData_isLoaded");
		}
	}
}
