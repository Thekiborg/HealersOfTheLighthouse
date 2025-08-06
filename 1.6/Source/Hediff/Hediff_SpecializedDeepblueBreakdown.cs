using AlienRace;

namespace HealersOfTheLighthouse
{
	public class Hediff_SpecializedDeepblueBreakdown : HediffWithComps
	{
		internal AbilityDef abilityDefToGive;
		internal GeneClass_SpecializedDeepblueBreakdown SDBDGene;


		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);

			if (abilityDefToGive is not null)
			{
				pawn.abilities.GainAbility(abilityDefToGive);
			}
		}


		public override void PreRemoved()
		{
			SDBDGene.currentDosage = 0;
			SDBDGene.isTransformed = false;
			pawn.abilities.RemoveAbility(abilityDefToGive);

			var alienComp = pawn.GetComp<AlienPartGenerator.AlienComp>();
			alienComp.OverwriteColorChannel("SDBDColor", Color.white);
			alienComp.RegenerateAddonsForced();

			base.PreRemoved();
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref abilityDefToGive, "HOTL_Hediff_SpecializedDeepblueBreakdown_AbilityToGive");
			Scribe_References.Look(ref SDBDGene, "HOTL_Hediff_SpecializedDeepblueBreakdown_SDBDGene");
		}
	}
}
