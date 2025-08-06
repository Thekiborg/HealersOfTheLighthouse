namespace HealersOfTheLighthouse
{
	public class RoomRoleWorker_Spa : RoomRoleWorker
	{
		private const float ScorePerBuilding = 50f;


		public override float GetScore(Room room)
		{
			int buildingCount = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			foreach (Thing thing in containedAndAdjacentThings)
			{
				if (thing.def == HOTL_ThingDefOfs.HOTL_MassageBed)
				{
					buildingCount++;
				}
			}
			return ScorePerBuilding * buildingCount;
		}
	}
}
