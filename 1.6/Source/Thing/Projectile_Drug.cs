namespace HealersOfTheLighthouse
{
	public class Projectile_Drug : Bullet
	{
		private ProjectileDrugSettings projDrugSettings;


		private ProjectileDrugSettings ProjectileDrugSettings
		{
			get
			{
				projDrugSettings ??= def?.GetModExtension<ModExtension>().projectileDrugSettings;
				return projDrugSettings;
			}
		}

		public override Quaternion ExactRotation
		{
			get
			{
				Quaternion rotationToAdd = Quaternion.AngleAxis(ProjectileDrugSettings.extraRotation, Vector3.up);
				return rotationToAdd * base.ExactRotation;
			}
		}


		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			base.Impact(hitThing, blockedByShield);
			if (hitThing is Pawn pawn)
			{
				Thing drug = ThingMaker.MakeThing(ProjectileDrugSettings.drugThingDef);
				drug?.Ingested(pawn, pawn.needs?.food?.NutritionWanted ?? 0f);
			}
		}
	}
}
