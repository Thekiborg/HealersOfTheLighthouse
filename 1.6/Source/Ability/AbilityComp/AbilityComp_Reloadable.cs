using Verse.Sound;

namespace HealersOfTheLighthouse
{
	public class AbilityComp_Reloadable : AbilityComp
	{
		public AbilityCompProperties_Reloadable Props => (AbilityCompProperties_Reloadable)props;

		private Pawn Caster => parent.pawn;
		public ThingDef AmmoDef => Props.ammoDef;
		public int BaseReloadTicks => Props.baseReloadTicks;
		public int MaxCharges => parent.maxCharges;
		public string LabelRemaining => $"{RemainingCharges} / {MaxCharges}";
		public int RemainingCharges => parent.RemainingCharges;


		public bool CanBeUsed(out string reason)
		{
			if (RemainingCharges <= 0)
			{
				reason = DisabledReason(MinAmmoNeeded(false), MaxAmmoNeeded(false));
				return false;
			}
			return parent.GizmoDisabled(out reason);
		}

		public string DisabledReason(int minNeeded, int maxNeeded)
		{
			return "CommandReload_NoAmmo".Translate(
				"charge".Named("CHARGENOUN"),
				AmmoDef.Named("AMMO"),
				(minNeeded == maxNeeded ? minNeeded.ToString() : $"{minNeeded}-{maxNeeded}").Named("COUNT"));
		}

		public int MaxAmmoAmount()
		{
			return Props.ammoCountPerCharge * MaxCharges;
		}

		public int MaxAmmoNeeded(bool allowForcedReload)
		{
			if (!NeedsReload(allowForcedReload))
			{
				return 0;
			}
			return Props.ammoCountPerCharge * (MaxCharges - RemainingCharges);
		}

		public int MinAmmoNeeded(bool allowForcedReload)
		{
			if (!NeedsReload(allowForcedReload))
			{
				return 0;
			}
			return Props.ammoCountPerCharge;
		}

		public bool NeedsReload(bool allowForceReload)
		{
			return RemainingCharges != MaxCharges;
		}

		public void ReloadFrom(Thing ammo)
		{
			if (!NeedsReload(true))
			{
				return;
			}
			if (ammo?.stackCount < Props.ammoCountPerCharge)
			{
				return;
			}
			int num = Mathf.Clamp(ammo.stackCount / Props.ammoCountPerCharge, 0, MaxCharges - RemainingCharges);
			ammo.SplitOff(num * Props.ammoCountPerCharge).Destroy();
			parent.RemainingCharges += num;

			Props.soundReload?.PlayOneShot(new TargetInfo(Caster.Position, Caster.Map));
		}
	}
}
