namespace HealersOfTheLighthouse
{
	public class GeneClass_PacifistNature : Gene
	{
		public void GiveThought(ThoughtDef thoughtToGive)
		{
			pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtToGive);
		}
	}
}
