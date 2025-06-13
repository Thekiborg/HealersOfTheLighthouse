using AlienRace;

namespace HealersOfTheLighthouse
{
    public class HediffComp_RapidMetabolicBreakDown : HediffComp
    {
        internal AbilityDef abilityDefToGive;
        internal Color bodyAddonColor;
        internal GeneClass_RapidMetabolicBreakdown RMBDGene;


		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);

            if (abilityDefToGive is not null)
            {
                Pawn.abilities.GainAbility(abilityDefToGive);
            }
		}


		public override void CompPostPostRemoved()
		{
			base.CompPostPostRemoved();
			RMBDGene.currentDosage = 0;
			RMBDGene.isTransformed = false;
			Pawn.abilities.RemoveAbility(abilityDefToGive);

			var alienComp = Pawn.GetComp<AlienPartGenerator.AlienComp>();
			alienComp.OverwriteColorChannel("RMBDColor", first: null);
			alienComp.RegenerateAddonsForced();
		}
    }
}
