namespace HealersOfTheLighthouse
{
	public class AbilityComp_Theorize : AbilityComp_DisableAfterUse
	{
		public new AbilityCompProperties_Theorize Props => (AbilityCompProperties_Theorize)props;


		public override bool GizmoDisabled(out string reason)
		{
			if (Find.ResearchManager.GetProject() is null)
			{
				reason = "HOTL_AbilityTheorize_GizmoDisabled_NoResearchProject".Translate();
				return true;
			}
			else
			{
				return base.GizmoDisabled(out reason);
			}
		}


		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.Pawn is not null)
			{
				if (target.Pawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
				{
					Messages.Message("HOTL_AbilityTheorize_IncapableOfIntellect".Translate(target.Pawn.Named("TARGET")), MessageTypeDefOf.RejectInput);
					return false;
				}
			}
			return base.CanApplyOn(target, dest);
		}
	}
}
