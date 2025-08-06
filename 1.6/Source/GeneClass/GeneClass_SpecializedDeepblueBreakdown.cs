using AlienRace;

namespace HealersOfTheLighthouse
{
	public class GeneClass_SpecializedDeepblueBreakdown : Gene
	{
		// --- Variables ---
		private ModExtension modExt;
		private SpecializedDeepblueBreakdown_Settings lockedSDBDSetting;
		private int settingIndex;

		internal int currentDosage;
		internal bool isTransformed;


		// --- Properties ---
		private ModExtension ModExt
		{
			get
			{
				modExt ??= def.GetModExtension<ModExtension>();
				return modExt;
			}
		}
		private ThingDef LockedDrugThingDef => lockedSDBDSetting.drugThingDef;
		private int LockedDosagesToTransform => lockedSDBDSetting.dosageToTransform;
		private Color LockedTransformationColor => lockedSDBDSetting.transformationColor;
		private AbilityDef LockedAbilityGiven => lockedSDBDSetting.abilityGiven;


		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			ThingDef drugThingDef = thing?.def;
			if (ModExt.SDBDSettings.NullOrEmpty() || isTransformed) return;

			// If it's taken the same drug as before
			if (lockedSDBDSetting is not null && drugThingDef == LockedDrugThingDef)
			{
				currentDosage++;
				TryTransform();
			}
			else
			{
				// If it's taken a different drug reset the dosages.
				for (int i = 0; i < ModExt.SDBDSettings.Count; i++)
				{
					var setting = ModExt.SDBDSettings[i];
					if (drugThingDef == setting.drugThingDef)
					{
						lockedSDBDSetting = setting;
						settingIndex = i;
						currentDosage = 1; // dosages are reset, but we're already taking a drug, so it's set to 1
						TryTransform();
						return;
					}
				}

				// If it's taken a different drug and it's not one of the options in the list, set to 0.
				currentDosage = 0;
			}
		}


		private void TryTransform()
		{
			if (currentDosage >= LockedDosagesToTransform)
			{
				Hediff_SpecializedDeepblueBreakdown hediff = HediffMaker.MakeHediff(ModExt.SDBDHediff, pawn) as Hediff_SpecializedDeepblueBreakdown;
				hediff.abilityDefToGive = LockedAbilityGiven;
				hediff.SDBDGene = this;

				pawn.health.AddHediff(hediff);
				currentDosage = 0;
				isTransformed = true;

				var alienComp = pawn.GetComp<AlienPartGenerator.AlienComp>();
				alienComp.OverwriteColorChannel("SDBDColor", first: LockedTransformationColor);
				alienComp.RegenerateAddonsForced();
			}
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref currentDosage, "HOTL_SpecializedDeepblueBreakdown_currentDosage");
			Scribe_Values.Look(ref isTransformed, "HOTL_SpecializedDeepblueBreakdown_isTranformed");
			Scribe_Values.Look(ref settingIndex, "HOTL_SpecializedDeepblueBreakdown_settingIndex", forceSave: true);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				lockedSDBDSetting = ModExt.SDBDSettings[settingIndex];
			}
		}
	}
}
