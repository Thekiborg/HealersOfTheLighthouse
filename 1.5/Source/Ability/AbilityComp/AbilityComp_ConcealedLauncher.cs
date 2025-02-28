using System.Linq;

namespace HealersOfTheLighthouse
{
	// FIND OUT HOW TO INDICATE A ROUND IS SELECTED BUT NOT LOADED
	// LOOK INTO HOW TO HANDLE CHANGING AMMO WHEN ITS ALREADY LOADED
	public class AbilityComp_ConcealedLauncher : CompAbilityEffect
	{
		// --- Fields ---
		private const int BurstSpreadRange = 5;
		private IReadOnlyList<ConcealedLauncherFireMode> fireModes;
		internal List<ConcealedLauncherMagazineData> magazine;
		private int fireModeIndex;
		private int magazineSlotToReload;


		// --- Properties ---
		public new AbilityCompProperties_ConcealedLauncher Props => (AbilityCompProperties_ConcealedLauncher)props;
		public int FireModeIndex { get => fireModeIndex; set => fireModeIndex = value; }
		public int FireModesCount => fireModes.Count;
		public ConcealedLauncherFireMode CurFireMode => fireModes[FireModeIndex];
		public int NumberOfUnloadedSlots => magazine.Count(slot => !slot.IsLoaded);
		public int MagazineSlotToReload => magazineSlotToReload;



		public override void Initialize(AbilityCompProperties props)
		{
			base.Initialize(props);
			magazine = [];
			for (int i = 0; i < Props.ammoCapacity; i++)
			{
				magazine.Add(new ConcealedLauncherMagazineData(null, false));
			}
			fireModes = GetFireModes();
		}


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);

			CurFireMode.Shoot(target, dest);
		}


		protected virtual IReadOnlyList<ConcealedLauncherFireMode> GetFireModes()
		{
			return new List<ConcealedLauncherFireMode>()
			{
				new("ConcealedLauncher_SingleFireMode",
					TextureLibrary.thinIcon,
					shotsPerBurst: 1,
					SingleShot),

				new("ConcealedLauncher_ClusterFireMode",
					TextureLibrary.fatIcon,
					shotsPerBurst: 5,
					(LocalTargetInfo target, LocalTargetInfo dest) =>
					{
						CellFinder.TryFindRandomCellNear(target.Cell, parent.pawn.Map, BurstSpreadRange, cell => !cell.Fogged(parent.pawn.Map), out IntVec3 cell);
						SingleShot(cell, cell);
					})
			};
		}


		private void SingleShot(LocalTargetInfo target, LocalTargetInfo dest)
		{
			for (int i = 0; i < magazine.Count; i++)
			{
				var slot = magazine[i];
				if (slot.ThingDef is not null && slot.IsLoaded)
				{
					Projectile projectile = (Projectile)GenSpawn.Spawn(slot.ThingDef.projectileWhenLoaded, parent.pawn.Position, parent.pawn.Map);
					projectile.Launch(parent.pawn, target, target, ProjectileHitFlags.NonTargetWorld);

					slot.IsLoaded = false;
					break;
				}
			}
		}


		public bool FindReloadableSlot()
		{
			for (int i = 0; i < magazine.Count; i++)
			{
				var slot = magazine[i];
				if (slot is null || (slot.ThingDef is not null && !slot.IsLoaded))
				{
					magazineSlotToReload = i;
					return true;
				}
			}
			return false;
		}


		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}

			if ((Props.displayGizmoWhileDrafted && parent.pawn.Drafted) || (Props.displayGizmoWhileUndrafted && !parent.pawn.Drafted))
			{
				yield return new ConcealedLauncherGizmo(this, magazine);
			}
		}


		public override bool GizmoDisabled(out string reason)
		{
			if (NumberOfUnloadedSlots >= magazine.Count)
			{
				reason = "ConcealedLauncher_Unloaded".Translate();
				return true;
			}
			return base.GizmoDisabled(out reason);
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Collections.Look(ref magazine, "HOTL_ConcealedLauncher_magazine", LookMode.Deep);
			Scribe_Values.Look(ref fireModeIndex, "HOTL_ConcealedLauncher_fireModeIndex");
			Scribe_Values.Look(ref magazineSlotToReload, "HOTL_ConcealedLauncher_magazineSlotToReload");
		}
	}
}
