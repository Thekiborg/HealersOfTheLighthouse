namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051

	public class TheorizeAbilitySettings
    {
		public IntRange chatBubbleDelay = new(300, 750);
        public int chatDuration = 10000;

        public float minSkillFactorOutput = 0.2f;
        public float maxSkillFactorOutput = 1.2f;
		public float minDiminishingOutput = 1.5f;
		public float maxDiminishingOutput = 0.65f;

        public InteractionDef interactionDef;
    }
}
