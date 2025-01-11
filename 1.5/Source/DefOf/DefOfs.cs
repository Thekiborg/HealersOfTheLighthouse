namespace HealersOfTheLighthouse
{
#pragma warning disable CA2211
    [DefOf]
    public static class MoyoMedicalExpansion_GeneDefOfs
    {
        static MoyoMedicalExpansion_GeneDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_GeneDefOfs));
        }

        public static GeneDef HOTL_RapidMetabolicBreakDown;
        public static GeneDef HOTL_PacifistNature;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_HediffDefOfs
    {
        static MoyoMedicalExpansion_HediffDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_HediffDefOfs));
        }

        public static HediffDef HOTL_RapidMetabolicBreakDown;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_ThoughtDefOfs
    {
        static MoyoMedicalExpansion_ThoughtDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_ThoughtDefOfs));
        }

        public static ThoughtDef HOTL_PacifistHarmedPawn;
        public static ThoughtDef HOTL_TheorizedWithColonist;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_JobDefOfs
    {
        static MoyoMedicalExpansion_JobDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_JobDefOfs));
        }

        public static JobDef HOTL_FollowFirstPawnToTheorize;
        public static JobDef HOTL_TakeSecondPawnToTheorize;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_AbilityDefOfs
    {
        static MoyoMedicalExpansion_AbilityDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_AbilityDefOfs));
        }

        public static AbilityDef HOTL_RMBD_AbilityTheorize;
    }
}
