namespace HealersOfTheLighthouse
{
    public class AbilityComp_Theorize : AbilityComp_DisableAfterUse
    {
        public override bool GizmoDisabled(out string reason)
        {
            if (Find.ResearchManager.GetProject() is null)
            {
                reason = "AbilityTheorize_GizmoDisabled_NoResearchProject".Translate();
                return true;
            }
            else
            {
                return base.GizmoDisabled(out reason);
            }
        }
    }
}
