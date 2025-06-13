using Verse.Grammar;

namespace HealersOfTheLighthouse
{
	public class Rule_CurrentResearch : Rule
	{
		public override float BaseSelectionWeight => Find.ResearchManager?.GetProject() != null ? 1 : 0;


		public override string Generate()
		{
			return Find.ResearchManager?.GetProject()?.LabelCap;
		}


		public override Rule DeepCopy()
		{
			Rule_CurrentResearch obj = (Rule_CurrentResearch)base.DeepCopy();
			return obj;
		}


		public override string ToString()
		{
			return keyword + "->(researchCurrent)";
		}
	}
}
