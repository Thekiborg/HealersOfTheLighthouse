namespace HealersOfTheLighthouse
{
#pragma warning disable CA1002, CA1051
	public class HediffCompProperties_CombatProsthetic : HediffCompProperties
	{
		public HediffCompProperties_CombatProsthetic()
		{
			compClass = typeof(HediffComp_CombatProsthetic);
		}

		public List<AbilityDef> combatAbilities;
	}
}
