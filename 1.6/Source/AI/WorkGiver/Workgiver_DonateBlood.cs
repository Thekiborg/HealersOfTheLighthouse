using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class Workgiver_DonateBlood : WorkGiver_Scanner
	{
		private BloodDonationSettings bloodDonoSettings;
		internal const float MinBloodlossSeverity = 0.45f;


		public BloodDonationSettings BloodDonation
		{
			get
			{
				bloodDonoSettings ??= def.GetModExtension<ModExtension>().bloodDonationSettings;
				return bloodDonoSettings;
			}
		}
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(HOTL_ThingDefOfs.HOTL_BloodDonationChair);
		public override PathEndMode PathEndMode => PathEndMode.OnCell;


		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return pawn?.Map.GetComponent<MapComponent_HOTL>().TrackedHemogen.Count >= GameComponent_HOTL.WantedHemogenCount;
		}


		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (pawn is null || !pawn.CanReserve(t))
			{
				return false;
			}
			if (!pawn.RaceProps.IsFlesh)
			{
				return false;
			}
			if (!pawn.DevelopmentalStage.Adult())
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) is not null)
			{
				return false;
			}
			CompForbiddable compForbiddable = t.TryGetComp<CompForbiddable>();
			if (compForbiddable is not null && compForbiddable.Forbidden)
			{
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			if (HasDisallowedGene(pawn))
			{
				return false;
			}
			Hediff bloodLoss = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
			if (bloodLoss is not null && bloodLoss.Severity <= MinBloodlossSeverity)
			{
				return false;
			}
			return true;
		}


		private bool HasDisallowedGene(Pawn pawn)
		{
			foreach (GeneDef geneDef in BloodDonation.disallowedGenes)
			{
				if (pawn.genes.HasActiveGene(geneDef))
				{
					return true;
				}
			}
			return false;
		}


		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_DonateBlood, t);
		}
	}
}
