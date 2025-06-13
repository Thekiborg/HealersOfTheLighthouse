namespace HealersOfTheLighthouse
{
	public class HediffComp_CombatProsthetic : HediffComp
	{
		public HediffCompProperties_CombatProsthetic Props => (HediffCompProperties_CombatProsthetic)props;


		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);

			if (!Props.combatAbilities.NullOrEmpty() )
			{
				foreach (AbilityDef abilityDef in Props.combatAbilities)
				{
					Pawn.abilities.abilities.Add(AbilityUtility.MakeAbility(abilityDef, Pawn));
					Pawn.abilities.Notify_TemporaryAbilitiesChanged();
				}
			}
		}


		public override void CompPostPostRemoved()
		{
			base.CompPostPostRemoved();

			if (!Props.combatAbilities.NullOrEmpty())
			{
				foreach (AbilityDef abilityDef in Props.combatAbilities)
				{
					Pawn.abilities.RemoveAbility(abilityDef);
				}
			}
		}
	}
}
