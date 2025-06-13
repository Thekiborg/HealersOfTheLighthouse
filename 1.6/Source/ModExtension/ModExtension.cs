namespace HealersOfTheLighthouse
{
#pragma warning disable CA1002, CA1051
    public class ModExtension : DefModExtension
    {
        public HediffDef RMBDHediff;
        public List<RapidMetabolicBreakdown_Settings> RMBDSettings;

        public MassageSettings massageSettings;

        public BloodDonationSettings bloodDonationSettings;

        public ProjectileDrugSettings projectileDrugSettings;

		public bool isRevealingApparel;
        public float severityToDisable;

    }
}
