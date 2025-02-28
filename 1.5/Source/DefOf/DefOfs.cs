namespace HealersOfTheLighthouse
{
#pragma warning disable CA2211
	[DefOf]
    public static class HealersOfTheLighthouse_GeneDefOfs
    {
        static HealersOfTheLighthouse_GeneDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HealersOfTheLighthouse_GeneDefOfs));
        }

        public static GeneDef HOTL_RapidMetabolicBreakDown;
        public static GeneDef HOTL_PacifistNature;
    }


    [DefOf]
    public static class HealersOfTheLighthouse_ThoughtDefOfs
    {
        static HealersOfTheLighthouse_ThoughtDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HealersOfTheLighthouse_ThoughtDefOfs));
        }

        public static ThoughtDef HOTL_PacifistHarmedPawn;
        public static ThoughtDef HOTL_TheorizedWithColonist;
    }


    [DefOf]
    public static class HealersOfTheLighthouse_JobDefOfs
    {
        static HealersOfTheLighthouse_JobDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HealersOfTheLighthouse_JobDefOfs));
        }

        public static JobDef HOTL_FollowFirstPawnToTheorize;
        public static JobDef HOTL_TakeSecondPawnToTheorize;
        public static JobDef HOTL_ConcealedArmament_ReloadLauncher;
	}


    [DefOf]
    public static class HealersOfTheLighthouse_AbilityDefOfs
    {
        static HealersOfTheLighthouse_AbilityDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HealersOfTheLighthouse_AbilityDefOfs));
        }

        public static AbilityDef HOTL_RMBD_AbilityTheorize;
        public static AbilityDef HOTL_ConcealedArmament_Launcher;
	}
}
