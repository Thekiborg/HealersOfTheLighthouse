
namespace HealersOfTheLighthouse
{
	public class AbilityComp_BoosterLauncher : AbilityComp_ConcealedLauncher
	{
		protected override IReadOnlyList<ConcealedLauncherFireMode> GetFireModes()
		{
			return new List<ConcealedLauncherFireMode>
			{
				new("ConcealedLauncher_SingleFireMode",
				TextureLibrary.marbleMedicineShotIcon,
				shotsPerBurst: 1,
				SingleShot)
			};
		}


		private void SingleShot(LocalTargetInfo target, LocalTargetInfo dest)
		{
			var slot = GetFirstLoadedSlot;
			Projectile projectile = (Projectile)GenSpawn.Spawn(slot.ThingDef.projectileWhenLoaded, parent.pawn.Position, parent.pawn.Map);
			projectile.Launch(parent.pawn, target, target, ProjectileHitFlags.IntendedTarget);

			slot.IsLoaded = false;
		}
	}
}
