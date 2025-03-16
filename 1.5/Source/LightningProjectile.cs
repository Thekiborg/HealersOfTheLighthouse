using System.Linq;

namespace HealersOfTheLighthouse
{
	public class LightningProjectile : Bullet
	{
		Mote mote;
		internal float coneRange;
		internal int coneAngleDegs;
		Thing lastHitThing;


		public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
		{
			mote = MoteMaker.MakeInteractionOverlay(HOTL_ThingDefOfs.Mote_GraserBeamBase, lastHitThing ?? launcher, usedTarget.ToTargetInfo(launcher?.Map));

			base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
		}

		public override void Tick()
		{
			base.Tick();
			mote?.Maintain();
		}

		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			if (hitThing is not null)
			{
				Vector3 origin = lastHitThing is null ? Launcher.Position.ToVector3Shifted().Yto0() : lastHitThing.Position.ToVector3Shifted().Yto0();
				Vector3 dest = hitThing.Position.ToVector3Shifted().Yto0();
				Pawn closestVictim = LightningArcUtility.GetPawnsForArc(origin, dest, Map, coneRange, coneAngleDegs)
									.Where(p => !p.DeadOrDowned)
									.OrderBy(p => p.Position.DistanceToSquared(hitThing.Position))
									.FirstOrDefault();

				if (closestVictim is not null)
				{
					LightningProjectile thing = ThingMaker.MakeThing(HOTL_ThingDefOfs.HOTL_LIGHTNINGBULLET) as LightningProjectile;
					thing.coneRange = coneRange;
					thing.coneAngleDegs = coneAngleDegs;
					thing.lastHitThing = hitThing;

					GenSpawn.Spawn(thing, hitThing.Position, hitThing.Map);
					thing.Launch(launcher, closestVictim, closestVictim, ProjectileHitFlags.IntendedTarget);
				}
			}
			base.Impact(hitThing, blockedByShield);
		}
	}
}
