using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class FloatMenuOptionProvider_ReloadConcealedLauncher : FloatMenuOptionProvider
	{
		private Ability concealedLauncherFound;
		private int slotToReload;


		protected override bool Undrafted => true;
		protected override bool Drafted => true;
		protected override bool Multiselect => false;


		// From reading ILSpy, it seems both Apply and AppliesInt run before GetOptions
		protected override bool AppliesInt(FloatMenuContext context)
		{
			concealedLauncherFound = context?.FirstSelectedPawn.abilities.abilities
				.FirstOrFallback((Ability a) => a.CompOfType<AbilityComp_ConcealedLauncher>()?.FindEmptySlot(out slotToReload) != false);

			return concealedLauncherFound is not null;
		}


		protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
		{
			AbilityComp_ConcealedLauncher concealedLauncherComp = concealedLauncherFound.CompOfType<AbilityComp_ConcealedLauncher>();

			if (clickedThing?.def == concealedLauncherComp?.Magazine[slotToReload].ThingDef)
			{
				if (!context.FirstSelectedPawn.CanReserve(clickedThing))
				{
					if (context.FirstSelectedPawn.MapHeld.reservationManager.TryGetReserver(clickedThing, context.FirstSelectedPawn.Faction, out var reserver) && reserver is not null)
					{
						return new FloatMenuOption(
							$"{"ConcealedLauncher_CantReload".Translate()}: {"ReservedBy".Translate(reserver.LabelShort, reserver).Resolve().StripTags()}",
							null);
					}
					else
					{
						return new FloatMenuOption(
							 $"{"ConcealedLauncher_CantReload".Translate()}: {"Reserved".Translate()}",
							 null);
					}
				}

				if (!context.FirstSelectedPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
				{
					return new FloatMenuOption(
						$"{"ConcealedLauncher_CantReload".Translate()}: {"IncapableOf".Translate().CapitalizeFirst()} {WorkTypeDefOf.Firefighter.gerundLabel}",
						null);
				}

				return new FloatMenuOption(
					"ConcealedLauncher_CanReload".Translate(),
					() =>
					{
						Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_ConcealedArmament_ReloadLauncher, clickedThing);
						job.count = 1;
						context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job);
					});
			}
			return null;
		}
	}
}
