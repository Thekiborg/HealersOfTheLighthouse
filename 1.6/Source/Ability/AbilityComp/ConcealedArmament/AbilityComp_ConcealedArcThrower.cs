namespace HealersOfTheLighthouse
{
	public class AbilityComp_ConcealedArcThrower : AbilityComp_ConcealedLightningArmament
	{
		private Pawn Caster => parent.pawn;


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			LightningProjectile thing = ThingMaker.MakeThing(HOTL_ThingDefOfs.HOTL_LightningArc) as LightningProjectile;
			thing.coneAngleDegs = Props.arcConeAngleDegs;
			thing.coneRange = Props.arcConeRange;
			thing.maxJumpCount = NumberOfJumps;
			thing.stunDurationTicks = Props.stunDurationTicks;

			GenSpawn.Spawn(thing, Caster.Position, Caster.Map);
			thing.Launch(Caster, target, target, ProjectileHitFlags.IntendedTarget);

			base.Apply(target, dest);
		}
	}
}
