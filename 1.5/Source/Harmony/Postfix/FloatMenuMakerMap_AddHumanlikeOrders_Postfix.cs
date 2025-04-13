using System.Linq;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	[HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
	internal static class FloatMenuMakerMap_AddHumanlikeOrders_Postfix
	{
		private static readonly List<Ability> concealedLauncherFound = [];

		#region Allow pawns with the concealed launchers to prioritize reload
		[HarmonyPostfix]
		internal static void ForcePawnToReload(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			int slotToReload = 0;

			concealedLauncherFound.Clear();
			concealedLauncherFound.AddRange(pawn.abilities.abilities.Where(ab => ab.CompOfType<AbilityComp_ConcealedLauncher>()?.FindEmptySlot(out slotToReload) != false));
			// Should get both marble launcher and booster launcher
			if (concealedLauncherFound.Count <= 0) return;

			AbilityComp_ConcealedLauncher concealedLauncher = concealedLauncherFound.First().CompOfType<AbilityComp_ConcealedLauncher>();
			if (concealedLauncher is null) return;


			IntVec3 clickCell = IntVec3.FromVector3(clickPos);
			foreach (Thing thing in clickCell.GetThingList(pawn.Map))
			{
				if (thing.def == concealedLauncher.Magazine[slotToReload].ThingDef)
				{
					if (!pawn.CanReserve(thing))
					{
						if (pawn.MapHeld.reservationManager.TryGetReserver(thing, pawn.Faction, out var reserver) && reserver is not null)
						{
							 opts.Add(new FloatMenuOption(
								 $"{"ConcealedLauncher_CantReload".Translate()}: {"ReservedBy".Translate(reserver.LabelShort, reserver).Resolve().StripTags()}",
								 null));
						}
						else
						{
							opts.Add(new FloatMenuOption(
								 $"{"ConcealedLauncher_CantReload".Translate()}: {"Reserved".Translate()}",
								 null));
						}
						continue;
					}

					if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
					{
						opts.Add(new FloatMenuOption(
							$"{"ConcealedLauncher_CantReload".Translate()}: {"IncapableOf".Translate().CapitalizeFirst()} {WorkTypeDefOf.Firefighter.gerundLabel}",
							null));
						continue;
					}

					opts.Add(new FloatMenuOption(
						"ConcealedLauncher_CanReload".Translate(),
						() =>
						{
							Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_ConcealedArmament_ReloadLauncher, thing);
							job.count = 1;
							pawn.jobs.TryTakeOrderedJob(job);
						}));
					return;
				}
			}
		}
		#endregion
	}
}
