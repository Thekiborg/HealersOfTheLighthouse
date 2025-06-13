namespace HealersOfTheLighthouse
{
	public class Recipe_RemoveExoarm : Recipe_RemoveBodyPart
	{
		protected override bool SpawnPartsWhenRemoved => false;


		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			foreach (BodyPartDef partDef in recipe?.appliedOnFixedBodyParts)
			{
				BodyPartRecord part = pawn?.health.hediffSet.GetBodyPartRecord(partDef);

				if (!pawn.Dead && pawn.health.hediffSet.hediffs.Any((Hediff hd) => hd.def == recipe.removesHediff && hd.Part == part))
				{
					yield return part;
				}
			}
		}


		public override void DamagePart(Pawn pawn, BodyPartRecord part) { }


		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			bool pawnDied = !pawn.Dead;
			bool violation = IsViolationOnPawn(pawn, part, Faction.OfPlayer);
			if (billDoer != null)
			{
				if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);

				Hediff foundHediff = null;
				for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
				{
					if (pawn.health.hediffSet.hediffs[i].Part == part)
					{
						foundHediff = pawn.health.hediffSet.hediffs[i];
					}
				}
				if (foundHediff != null)
				{
					foundHediff.Notify_SurgicallyRemoved(billDoer);
					pawn.health.RemoveHediff(foundHediff);
				}
			}
			DamagePart(pawn, part);
			pawn.Drawer.renderer.SetAllGraphicsDirty();
			if (pawnDied)
			{
				ApplyThoughts(pawn, billDoer);
			}
			if (violation)
			{
				ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
			}
		}


		public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
		{
			return recipe.label;
		}
	}
}
