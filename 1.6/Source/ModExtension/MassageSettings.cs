﻿namespace HealersOfTheLighthouse
{
	public class MassageSettings
	{
		public List<ThingDef> oils;
		public ThoughtDef oilThought;

		private const float BeautyMinNormValue = 0.5f;
		private const float BeautyMaxNormValue = 1.5f;
		private const int BeautyMin = -4;
		private const int BeautyMax = 4;

		private const float OpinionMinNormValue = 0.3f;
		private const float OpinionMaxNormValue = 1.7f;
		private const int OpinionMin = -100;
		private const int OpinionMax = 100;

		private const float MaxManipulationAllowed = 5f;

		private const float SocialMinNormValue = 0.3f;
		private const float SocialMaxNormValue = 1.7f;
		private const int SocialMin = 0;
		private const int SocialMax = 20;

		private const float baseJoyGainFactor = 2f;

		internal static float CalculateJoyFactor(Pawn firstPawn, Pawn secondPawn, bool firstIsTop)
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
