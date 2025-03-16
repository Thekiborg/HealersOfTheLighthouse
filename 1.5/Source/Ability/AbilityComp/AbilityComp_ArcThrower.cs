using System.Linq;

namespace HealersOfTheLighthouse
{
	public class AbilityComp_ArcThrower : CompAbilityEffect
	{
		public new AbilityCompProperties_LightningProjectile Props => (AbilityCompProperties_LightningProjectile)props;
		private Pawn Caster => parent.pawn;


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			LightningProjectile thing = ThingMaker.MakeThing(HOTL_ThingDefOfs.HOTL_LIGHTNINGBULLET) as LightningProjectile;
			thing.coneAngleDegs = Props.coneAngleDegs;
			thing.coneRange = Props.coneRange;

			GenSpawn.Spawn(thing, Caster.Position, Caster.Map);
			thing.Launch(Caster, target, target, ProjectileHitFlags.IntendedTarget);

			base.Apply(target, dest);
		}
	}
}
