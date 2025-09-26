namespace HealersOfTheLighthouse
{
	[HarmonyPatch(typeof(PawnCapacityWorker_Moving), nameof(PawnCapacityWorker_Moving.CalculateCapacityLevel))]
	internal static class PawnCapacityWorker_Moving_CalculateCapacityLevel_Postfix
	{
		[HarmonyPostfix]
		private static void IncreaseMovementIfWearingWheelchair(ref float __result, HediffSet diffSet)
		{
			if (GameComponent_HOTL.pawnsOnWheelchairs.Contains(diffSet.pawn))
			{
				__result = 0.8f * MathUtils.Normalization01(diffSet.pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation), 0f, 1f);
			}
		}
	}
}
