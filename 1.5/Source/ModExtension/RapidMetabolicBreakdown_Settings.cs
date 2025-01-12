namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
    public class RapidMetabolicBreakdown_Settings
    {
        public ThingDef drugThingDef; // The def of the drug that will cause a transformation
        public int dosageToTransform; // Counter of times the drug will need to be taken for the transformation to take place
        public Color transformationColor; // The color of the veins and horns when the pawn is transformed
        public AbilityDef abilityGiven; // Ability given to the pawn when transformed
    }
}