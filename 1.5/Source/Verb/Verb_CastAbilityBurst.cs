namespace HealersOfTheLighthouse
{
	public class Verb_CastAbilityBurst : Verb_CastAbility
	{
		AbilityComp_ConcealedLauncher CompCL;


		protected override int ShotsPerBurst
		{
			get
			{
				if (verbTracker.directOwner is Ability ability)
				{
					CompCL ??= ability.CompOfType<AbilityComp_ConcealedLauncher>();

					if (CompCL is not null)
					{
						return CompCL.CurFireMode.ShotsPerBurst - CompCL.NumberOfUnloadedSlots;
					}
				}
				return 1;
			}
		}
	}
}
