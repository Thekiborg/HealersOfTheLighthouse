using System.Linq;

namespace HealersOfTheLighthouse
{
	public abstract class AbilityComp_ConcealedLauncher : CompAbilityEffect
	{
		// --- Fields ---
		private IReadOnlyList<ConcealedLauncherFireMode> fireModes;
		private List<ConcealedLauncherMagazineData> magazine;
		private int fireModeIndex;


		// --- Properties ---
		public virtual new AbilityCompProperties_ConcealedLauncher Props => (AbilityCompProperties_ConcealedLauncher)props;
		public int FireModeIndex { get => fireModeIndex; set => fireModeIndex = value; }
		public int FireModesCount => fireModes.Count;
		public ConcealedLauncherFireMode CurFireMode => fireModes[FireModeIndex];
		public int NumberOfUnloadedSlots => magazine.Count(slot => !slot.IsLoaded);
		public IReadOnlyList<ConcealedLauncherMagazineData> Magazine => magazine;
		public ConcealedLauncherMagazineData GetFirstLoadedSlot
		{
			get
			{
				if (NumberOfUnloadedSlots >= Props.ammoCapacity) return null;
				foreach (ConcealedLauncherMagazineData slot in magazine)
				{
					if (slot.ThingDef is not null && slot.IsLoaded)
					{
						return slot;
					}
				}
				return null;
			}
		}


		protected abstract IReadOnlyList<ConcealedLauncherFireMode> GetFireModes();


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


		public bool FindEmptySlot(out int slotToReload)
		{
			for (int i = 0; i < magazine.Count; i++)
			{
				var slot = magazine[i];
				if (slot is null || (slot.ThingDef is not null && !slot.IsLoaded))
				{
					slotToReload = i;
					return true;
				}
			}
			slotToReload = int.MinValue;
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
			if (NumberOfUnloadedSlots >= Props.ammoCapacity)
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
		}
	}
}
