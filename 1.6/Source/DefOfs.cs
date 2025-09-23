namespace HealersOfTheLighthouse
{
#pragma warning disable CA2211
	[DefOf]
	public static class HOTL_HediffDefOfs
	{
		static HOTL_HediffDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_HediffDefOfs));
		}

		public static HediffDef HOTL_Installed_ExoArm;
	}


	[DefOf]
	public static class HOTL_GeneDefOfs
	{
		static HOTL_GeneDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_GeneDefOfs));
		}

		public static GeneDef HOTL_SpecialiedDeepblueBreakdown;
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
		public static ThoughtDef HOTL_HuggedColonist;
	}


	[DefOf]
	public static class HOTL_JobDefOfs
	{
		static HOTL_JobDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_JobDefOfs));
		}

		public static JobDef HOTL_FollowFirstPawnToTheorize;
		public static JobDef HOTL_GetSecondPawnAndTheorize;
		public static JobDef HOTL_ConcealedArmament_ReloadLauncher;
		public static JobDef HOTL_ConcealedArmament_ReloadAbility;
		public static JobDef HOTL_DonateBlood;
		public static JobDef HOTL_TopMassage;
		public static JobDef HOTL_BottomMassage;
		public static JobDef HOTL_MovePatientToOperatingBed;
		public static JobDef HOTL_ClearOperatingBed;
	}


	[DefOf]
	public static class HOTL_AbilityDefOfs
	{
		static HOTL_AbilityDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_AbilityDefOfs));
		}

		public static AbilityDef HOTL_SDBD_AbilityTheorize;
		public static AbilityDef HOTL_SDBD_AbilityHug;
		public static AbilityDef HOTL_ConcealedArmament_MarbleLauncher;
	}


	[DefOf]
	public static class HOTL_ThingDefOfs
	{
		static HOTL_ThingDefOfs()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(HOTL_ThingDefOfs));
		}

		public static ThingDef HOTL_LightningArc;
		public static ThingDef Mote_GraserBeamBase;
		public static ThingDef HOTL_BloodDonationChair;
		public static ThingDef HOTL_MassageBed;
		public static ThingDef HOTL_OperatingBed;
	}
}
