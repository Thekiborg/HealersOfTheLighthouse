﻿namespace HealersOfTheLighthouse
{
#pragma warning disable CA2211
	[DefOf]
    public static class HOTL_GeneDefOfs
    {
        static HOTL_GeneDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_GeneDefOfs));
        }

        public static GeneDef HOTL_RapidMetabolicBreakDown;
        public static GeneDef HOTL_PacifistNature;
    }


    [DefOf]
    public static class HOTL_ThoughtDefOfs
    {
        static HOTL_ThoughtDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_ThoughtDefOfs));
        }

        public static ThoughtDef HOTL_PacifistHarmedPawn;
        public static ThoughtDef HOTL_TheorizedWithColonist;
    }


    [DefOf]
    public static class HOTL_JobDefOfs
    {
        static HOTL_JobDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_JobDefOfs));
        }

        public static JobDef HOTL_FollowFirstPawnToTheorize;
        public static JobDef HOTL_TakeSecondPawnToTheorize;
        public static JobDef HOTL_ConcealedArmament_ReloadLauncher;
	}


    [DefOf]
    public static class HOTL_AbilityDefOfs
    {
        static HOTL_AbilityDefOfs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_AbilityDefOfs));
        }

        public static AbilityDef HOTL_RMBD_AbilityTheorize;
        public static AbilityDef HOTL_ConcealedArmament_Launcher;
	}


	[DefOf]
	public static class HOTL_ThingDefOfs
	{
		static HOTL_ThingDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_ThingDefOfs));
		}

		public static ThingDef HOTL_LIGHTNINGBULLET;
        public static ThingDef Mote_GraserBeamBase;
	}
}
