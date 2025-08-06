namespace HealersOfTheLighthouse
{
	public abstract class AbilityComp_ConcealedFireArmament : CompAbilityEffect, IFireAbility
	{
		private Pawn Caster => parent.pawn;


		public override bool GizmoDisabled(out string reason)
		{
			if (IsPawnWet())
			{
				reason = "ConcealedLauncher_WetDisabledReason".Translate();
				return true;
			}
			return base.GizmoDisabled(out reason);
		}

		public bool IsPawnWet()
		{
			return Caster?.needs?.mood?.thoughts?.memories?.GetFirstMemoryOfDef(ThoughtDefOf.SoakingWet) is not null;
		}
	}
}
