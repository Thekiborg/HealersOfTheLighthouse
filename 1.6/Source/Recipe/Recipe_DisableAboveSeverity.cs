using System.Linq;

namespace HealersOfTheLighthouse
{
	public class Recipe_DisableAboveSeverity : Recipe_InstallImplant
	{
		private ModExtension extension;


		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
			{
				if (!pawn.health.hediffSet.GetNotMissingParts().Contains(record))
				{
					return false;
				}
				if (pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record))
				{
					return false;
				}
				return !pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && !recipe.CompatibleWithHediff(x.def));
			});
		}


		public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
		{
			if (!base.AvailableOnNow(thing, part))
			{
				return false;
			}
			else
			{
				extension ??= recipe.GetModExtension<ModExtension>();
				if (((Pawn)thing)?.health?.hediffSet?.GetFirstHediffOfDef(recipe.addsHediff)?.Severity >= extension?.severityToDisable)
				{
					return false;
				}
			}
			return true;
		}
	}
}