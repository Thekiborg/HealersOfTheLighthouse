using System.Linq;

namespace HealersOfTheLighthouse
{
	public class AbilityComp_MarbleLauncher : AbilityComp_ConcealedLauncher
	{
		protected override IReadOnlyList<ConcealedLauncherFireMode> GetFireModes()
		{
			return new List<ConcealedLauncherFireMode>()
			{
				new("ConcealedLauncher_SingleFireMode",
					TextureLibrary.marbleSingleShotIcon,
					shotsPerBurst: 1,
					SingleShot),

				new("ConcealedLauncher_ClusterFireMode",
					TextureLibrary.marbleClusterShotIcon,
					shotsPerBurst: 5,
					(LocalTargetInfo target, LocalTargetInfo dest) =>
					{
						CellFinder.TryFindRandomCellNear(target.Cell, parent.pawn.Map, Props.burstSpreadRadiusSquared, cell => !cell.Fogged(parent.pawn.Map), out IntVec3 cell);
						SingleShot(cell, cell);
					})
			};
		}


		private void SingleShot(LocalTargetInfo target, LocalTargetInfo dest)
		{
			var slot = GetFirstLoadedSlot;
			Projectile projectile = (Projectile)GenSpawn.Spawn(slot.ThingDef.projectileWhenLoaded, parent.pawn.Position, parent.pawn.Map);
			projectile.Launch(parent.pawn, target, target, ProjectileHitFlags.NonTargetWorld);

			slot.IsLoaded = false;
		}


		public override void DrawEffectPreview(LocalTargetInfo target)
		{
			base.DrawEffectPreview(target);
			List<IntVec3> cells = [];
			var round = GetFirstLoadedSlot.ThingDef;
			float radius;

			if (round is null) return;

			switch (FireModeIndex)
			{
				case 0: // Single
					radius = round.projectileWhenLoaded.projectile.explosionRadius + round.projectileWhenLoaded.projectile.explosionRadiusDisplayPadding;
					cells = GenRadial.RadialCellsAround(target.Cell, radius, true).ToList();
					break;

				case 1: // Cluster 
					Log.Message("Hello");
					radius = Props.burstSpreadRadiusSquared + 1;
					cells = GenRadial.RadialCellsAround(target.Cell, radius, true).ToList();
					break;
			}

			GenDraw.DrawFieldEdges(cells);
		}


		public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
		{
			base.DrawEffectPreview(target);
			var round = GetFirstLoadedSlot.ThingDef;

			if (round == null) return base.ExtraLabelMouseAttachment(target);

			return FireModeIndex switch
			{
				// Single
				0 => (string)round.LabelCap,

				// Cluster
				/*
				1 => null, 
				*/

				_ => base.ExtraLabelMouseAttachment(target),
			};
		}
	}
}
