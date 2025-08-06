namespace HealersOfTheLighthouse
{
	public class HediffCompProperties_CombatProsthetic : HediffCompProperties
	{
		public HediffCompProperties_CombatProsthetic()
		{
			compClass = typeof(HediffComp_CombatProsthetic);
		}

		public List<AbilityDef> combatAbilities;
	}
}
