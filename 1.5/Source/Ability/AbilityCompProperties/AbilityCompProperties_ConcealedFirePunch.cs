namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class AbilityCompProperties_ConcealedFirePunch : CompProperties_AbilityEffect
	{
		public AbilityCompProperties_ConcealedFirePunch()
		{
			compClass = typeof(AbilityComp_ConcealedFirePunch);
		}

		public float baseKnockbackLength;
		public EffecterDef punchEffecter;
		public XMLDamageInfoSettings punchDamageInfo;
		public XMLDamageInfoSettings collideDamageInfo;
		public ThingDef pawnFlyerDef;
	}
}
