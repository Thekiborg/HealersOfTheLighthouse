using System.Linq;

namespace HealersOfTheLighthouse
{
	// DO 3 FIRE MODES, SINGLE BURST CLUSTER
	// CHANGE GIZMO COLOR WHEN HOVERING OVER SHOOT BUTTON
	// FIND OUT HOW TO INDICATE A ROUND IS SELECTED BUT NOT LOADED
	// FIX HOVERING OVER GIZMO DOES NOT SHOW THE RANGE
	public class AbilityComp_ConcealedLauncher : CompAbilityEffect
	{
		(ThingDef ThingDef, bool IsLoaded)[] magazine;


		public new AbilityCompProperties_ConcealedLauncher Props => (AbilityCompProperties_ConcealedLauncher)props;


		Pawn Caster => parent.pawn;


		public override void Initialize(AbilityCompProperties props)
		{
			base.Initialize(props);
			magazine = new (ThingDef ThingDef, bool IsLoaded)[Props.ammoCapacity];
		}


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			for (int i = 0; i < magazine.Length; i++)
			{
				var slot = magazine[i];
				if (slot.ThingDef is not null && slot.IsLoaded)
				{
					Projectile projectile = (Projectile)GenSpawn.Spawn(magazine[i].ThingDef.projectileWhenLoaded, parent.pawn.Position, parent.pawn.Map);
					projectile.Launch(parent.pawn, target, target, ProjectileHitFlags.NonTargetWorld);

					magazine[i].IsLoaded = false;
					break;
				}
			}
		}


		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}

			yield return new ConcealedLauncherGizmo(parent, Caster, magazine);
		}


		public override bool GizmoDisabled(out string reason)
		{
			int numberOfEmtpySlots = magazine.Count(slot => slot.ThingDef is null || !slot.IsLoaded);
			if (numberOfEmtpySlots == Props.ammoCapacity)
			{
				reason = "Hola".Translate();
				return true;
			}
			return base.GizmoDisabled(out reason);
		}
	}
}
