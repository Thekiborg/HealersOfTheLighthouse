namespace HealersOfTheLighthouse
{
	public class AbilityComp_Hug : AbilityComp_DisableAfterUse
	{
		public new AbilityCompProperties_Hug Props => (AbilityCompProperties_Hug)props;


		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.Pawn is not null)
			{
				if (!target.Pawn.InMentalState)
				{
					Messages.Message("HOTL_AbilityHug_CantDo".Translate(target.Pawn.Named("TARGET")), MessageTypeDefOf.RejectInput);
					return false;
				}
			}
			return base.CanApplyOn(target, dest);
		}
	}
}
