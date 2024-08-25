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
    }

    [DefOf]
    public static class MoyoMedicalExpansion_JobDefOfs
    {
        static MoyoMedicalExpansion_JobDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MoyoMedicalExpansion_JobDefOfs));
        }

        public static JobDef Thek_FollowFirstPawnToBrainstorm;
        public static JobDef Thek_TakeSecondPawnToBrainstorm;
    }
}
