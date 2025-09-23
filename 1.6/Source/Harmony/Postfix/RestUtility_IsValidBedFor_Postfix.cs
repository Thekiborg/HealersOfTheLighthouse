namespace HealersOfTheLighthouse
{
	[HarmonyPatch(typeof(RestUtility), nameof(RestUtility.IsValidBedFor))]
	internal static class RestUtility_IsValidBedFor_Postfix
	{
		[HarmonyPostfix]
		private static void OperatingBedsNotValidWithoutSurgeryPlanned(ref bool __result, Thing bedThing, Pawn sleeper)
		{
			if (bedThing.def == HOTL_ThingDefOfs.HOTL_OperatingBed && !sleeper.BillStack.AnyShouldDoNow)
			{
				__result = false;
			}
		}
	}
}
