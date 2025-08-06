namespace HealersOfTheLighthouse
{
	public abstract class AbilityComp_DisableAfterUse : CompAbilityEffect
	{
		protected bool abilityUsed;


		public override bool GizmoDisabled(out string reason)
		{
			reason = "AbilitySDBD_GizmoDisabled_Used".Translate();
			return abilityUsed;
		}


		public void AbilityUsed()
		{
			abilityUsed = true;
		}


		public override void PostExposeData()
		{
			Scribe_Values.Look(ref abilityUsed, "HOTL_SDBDAbilities_abilityUsed");
		}
	}
}
