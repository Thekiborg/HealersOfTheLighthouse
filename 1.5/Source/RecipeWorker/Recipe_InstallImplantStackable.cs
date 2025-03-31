using System.Linq;

namespace HealersOfTheLighthouse
{
	public class Recipe_InstallImplantStackable : Recipe_InstallImplant
	{
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
	}
}
