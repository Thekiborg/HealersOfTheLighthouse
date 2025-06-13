namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class AbilityCompProperties_ConcealedPropelledArm : CompProperties_AbilityEffect
	{
		public AbilityCompProperties_ConcealedPropelledArm()
		{
			compClass = typeof(AbilityComp_ConcealedPropelledArm);
		}

		public float baseKnockbackLength;
		public EffecterDef ArmEffecter;
		public XMLDamageInfoSettings ArmDamageInfo;
		public XMLDamageInfoSettings collideDamageInfo;
		public ThingDef pawnFlyerDef;
	}
}
