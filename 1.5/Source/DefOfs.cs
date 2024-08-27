namespace MoyoMedicalExpansion
{
#pragma warning disable CA2211
    [DefOf]
    public static class MoyoMedicalExpansion_GeneDefOfs
    {
        static MoyoMedicalExpansion_GeneDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_GeneDefOfs));
        }

        public static GeneDef Thek_RapidMetabolicBreakDown;
        public static GeneDef Thek_PacifistNature;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_HediffDefOfs
    {
        static MoyoMedicalExpansion_HediffDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_HediffDefOfs));
        }

        public static HediffDef Thek_RapidMetabolicBreakDown;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_ThoughtDefOfs
    {
        static MoyoMedicalExpansion_ThoughtDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_ThoughtDefOfs));
        }

        public static ThoughtDef Thek_PacifistHarmedPawn;
        public static ThoughtDef Thek_DiscussedWithSomeone;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_JobDefOfs
    {
        static MoyoMedicalExpansion_JobDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_JobDefOfs));
        }

        public static JobDef Thek_FollowFirstPawnToTheorize;
        public static JobDef Thek_TakeSecondPawnToTheorize;
    }


    [DefOf]
    public static class MoyoMedicalExpansion_AbilityDefOfs
    {
        static MoyoMedicalExpansion_AbilityDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_AbilityDefOfs));
        }

        public static AbilityDef Thek_RMBD_AbilityTheorize;
    }
}
