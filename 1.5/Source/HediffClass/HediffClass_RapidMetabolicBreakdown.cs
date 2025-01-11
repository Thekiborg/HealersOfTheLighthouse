using AlienRace;

namespace HealersOfTheLighthouse
{
    public class HediffClass_RapidMetabolicBreakdown : HediffWithComps
    {
        internal AbilityDef abilityDefToGive;
        internal Color bodyAddonColor;
        internal GeneClass_RapidMetabolicBreakdown RMBDGene;

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
            base.PreRemoved();
            RMBDGene.currentDosage = 0;
            RMBDGene.isTransformed = false;
            pawn.abilities.RemoveAbility(abilityDefToGive);

            var alienComp = pawn.GetComp<AlienPartGenerator.AlienComp>();
            alienComp.OverwriteColorChannel("RMBDColor", first: null);
            alienComp.RegenerateAddonsForced();
        }
    }
}
