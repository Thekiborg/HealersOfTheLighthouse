namespace HealersOfTheLighthouse
{
	public class AbilityComp_ConcealedLauncher : CompAbilityEffect
	{
		Pawn Caster => parent.pawn;


		internal (ThingDef ThingDef, bool IsLoaded)[] launcherMagazine;


		public new AbilityCompProperties_ConcealedLauncher Props => (AbilityCompProperties_ConcealedLauncher)props;


		public override void Initialize(AbilityCompProperties props)
		{
			base.Initialize(props);
			launcherMagazine = new (ThingDef ThingDef, bool IsLoaded)[Props.ammoCapacity];
		}


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Projectile projectile = (Projectile)GenSpawn.Spawn(launcherMagazine[0].ThingDef.projectileWhenLoaded, parent.pawn.Position, parent.pawn.Map);
			projectile.Launch(parent.pawn, target, target, ProjectileHitFlags.NonTargetWorld);
			
		}


		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}

			yield return new ConcealedLauncherGizmo(parent, Caster);
		}
	}
}
