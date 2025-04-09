using System.Linq;

namespace HealersOfTheLighthouse
{
#pragma warning disable CA1002, CA1051
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

		internal static float CalculateJoyFactor(float extraJoyGainFactor, Pawn firstPawn, Pawn secondPawn, bool firstIsTop)
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
				return extraJoyGainFactor * beautyF * opinionF * manF * socialF;
			}
			else
			{
				return extraJoyGainFactor * beautyF * opinionF;
			}
		}
	}
}
