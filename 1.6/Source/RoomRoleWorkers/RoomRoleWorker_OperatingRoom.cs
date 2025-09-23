namespace HealersOfTheLighthouse
{
	public class RoomRoleWorker_OperatingRoom : RoomRoleWorker
	{
		private const float HospitalScore = 100000f;
		private const float ScorePerBuilding = 100f;

		public override float GetScore(Room room)
		{
			int buildingCount = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			foreach (Thing thing in containedAndAdjacentThings)
			{
				if (thing.def == HOTL_ThingDefOfs.HOTL_OperatingBed)
				{
					buildingCount++;
					break;
				}
			}
			return (HospitalScore + ScorePerBuilding) * buildingCount;
		}
	}
}
