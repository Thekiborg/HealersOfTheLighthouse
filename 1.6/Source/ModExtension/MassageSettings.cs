namespace HealersOfTheLighthouse
{
	public class MassageSettings
	{
		public List<ThingDef> oils;
		public ThoughtDef oilThought;

		public float BeautyMinNormValue;
		public float BeautyMaxNormValue;
		public int BeautyMin;
		public int BeautyMax;

		public float OpinionMinNormValue;
		public float OpinionMaxNormValue;
		public int OpinionMin;
		public int OpinionMax;

		public float MaxManipulationAllowed;

		public float SocialMinNormValue;
		public float SocialMaxNormValue;
		public int SocialMin;
		public int SocialMax;

		public float baseJoyGainFactor;

		internal float CalculateJoyFactor(Pawn firstPawn, Pawn secondPawn, bool firstIsTop)
		{
			float clampedBeauty = Mathf.Clamp(secondPawn.GetStatValue(StatDefOf.PawnBeauty), BeautyMin, BeautyMax);
			float beautyF = MathUtils.NormalizationCustom(clampedBeauty, BeautyMin, BeautyMax, BeautyMinNormValue, BeautyMaxNormValue);
			if (!RelationsUtility.AttractedToGender(firstPawn, secondPawn.gender))
			{
				beautyF /= 2;
			}

			float clampedOpinion = Mathf.Clamp(firstPawn.relations.OpinionOf(secondPawn), OpinionMin, OpinionMax);
			float opinionF = MathUtils.NormalizationCustom(clampedOpinion, OpinionMin, OpinionMax, OpinionMinNormValue, OpinionMaxNormValue);


			if (!firstIsTop)
			{
				float manF = Mathf.Min(secondPawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation), MaxManipulationAllowed);

				float clampedSocial = Mathf.Clamp(secondPawn.skills.GetSkill(SkillDefOf.Social).Level, SocialMin, SocialMax);
				float socialF = MathUtils.NormalizationCustom(clampedSocial, SocialMin, SocialMax, SocialMinNormValue, SocialMaxNormValue);
				//Log.Message($"For: {firstPawn}. Beauty is {clampedBeauty} and factor is {beautyF}. Matches gender? {RelationsUtility.AttractedToGender(firstPawn, secondPawn.gender)}");
				//Log.Message($"Opinion is {clampedOpinion} and factor is {opinionF}");
				//Log.Message($"Manipulation is {manF} and social is {clampedSocial} and social factor is {socialF}");
				//Log.Message($"From {extraJoyGainFactor} we get {extraJoyGainFactor * beautyF * opinionF * manF * socialF}");
				return baseJoyGainFactor * beautyF * opinionF * manF * socialF;
			}
			else
			{
				//Log.Message($"For: {firstPawn}. Beauty is {clampedBeauty} and factor is {beautyF}. Matches gender? {RelationsUtility.AttractedToGender(firstPawn, secondPawn.gender)}");
				//Log.Message($"Opinion is {clampedOpinion} and factor is {opinionF}");
				//Log.Message($"From {extraJoyGainFactor} we get {extraJoyGainFactor * beautyF * opinionF}");
				//Log.Message(extraJoyGainFactor * beautyF * opinionF);
				return baseJoyGainFactor * beautyF * opinionF;
			}
		}
	}
}
