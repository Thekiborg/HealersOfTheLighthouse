namespace HealersOfTheLighthouse
{
	public class TrackerComp_Hemogen : ThingComp
	{
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			for (int i = 0; i < parent.stackCount; i++)
			{
				parent.Map.GetComponent<MapComponent_HOTL>().TrackedHemogen.Add(parent);
			}
		}


		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			base.PreAbsorbStack(otherStack, count);
			parent.Map.GetComponent<MapComponent_HOTL>().TrackedHemogen.Add(parent);
		}


		public override void PostDeSpawn(Map map)
		{
			map?.GetComponent<MapComponent_HOTL>().TrackedHemogen.RemoveAll(hemoInstance => hemoInstance == parent);
			base.PostDeSpawn(map);
		}


		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			previousMap?.GetComponent<MapComponent_HOTL>().TrackedHemogen.RemoveAll(hemoInstance => hemoInstance == parent);
			base.PostDestroy(mode, previousMap);
		}
	}
}
