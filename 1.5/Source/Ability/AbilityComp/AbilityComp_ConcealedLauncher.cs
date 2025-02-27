using System.Linq;
using UnityEngine.Networking.Types;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	// DO 3 FIRE MODES, SINGLE BURST CLUSTER
	//		Virtual array manually filled with classes with settings
	//		Right clicking button will change a counter in this class
	//		Different effects called depending on that counter
	// FIND OUT HOW TO INDICATE A ROUND IS SELECTED BUT NOT LOADED
	public class AbilityComp_ConcealedLauncher : CompAbilityEffect
	{
		// --- Fields ---
		private IReadOnlyList<ConcealedLauncherFireMode> fireModes;
		private List<ConcealedLauncherMagazineData> magazine;
		private int fireModeIndex;


		// --- Properties ---
		public new AbilityCompProperties_ConcealedLauncher Props => (AbilityCompProperties_ConcealedLauncher)props;
		public int FireModeIndex { get => fireModeIndex; set => fireModeIndex = value; }
		public int FireModesCount => fireModes.Count;
		public ConcealedLauncherFireMode CurFireMode => fireModes[FireModeIndex];
		private int NumberOfEmptyOrUnloadedSlots => magazine.Count(slot => slot is null || !slot.IsLoaded);



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
				new("Single",
					TextureLibrary.thinIcon,
					SingleShot),

				new("Cluster",
					TextureLibrary.fatIcon,
					(LocalTargetInfo target, LocalTargetInfo dest) =>
					{
						SingleShot(target, dest);
						if (NumberOfEmptyOrUnloadedSlots < Props.ammoCapacity)
						{
							Log.Message("HElo");
							Job job = JobMaker.MakeJob(JobDefOf.UseVerbOnThing, parent.pawn);
							job.verbToUse = parent.verb;
							parent.pawn.jobs.StartJob(job, JobCondition.InterruptForced);
						}
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


		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}

			yield return new ConcealedLauncherGizmo(this, magazine);
		}


		public override bool GizmoDisabled(out string reason)
		{
			if (NumberOfEmptyOrUnloadedSlots >= Props.ammoCapacity)
			{
				reason = "Hola".Translate();
				return true;
			}
			return base.GizmoDisabled(out reason);
		}


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Collections.Look(ref magazine, "HOTL_ConcealedLauncher_magazine", LookMode.Deep);
			Scribe_Values.Look(ref fireModeIndex, "HOTL_ConcealedLauncher_fireModeIndex");
		}
	}
}
