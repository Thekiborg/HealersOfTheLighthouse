using System.Linq;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobGiver_ReloadAbility : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn is null || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)) return null;

			Ability ability = FindAbilityNeedingReload(pawn, out var reloadableComp);

			if (reloadableComp is null) return null;

			if (pawn.carryTracker.AvailableStackSpace(reloadableComp.AmmoDef) < reloadableComp.MinAmmoNeeded(true))
			{
				return null;
			}
			List<Thing> foundAmmo = FindEnoughAmmo(pawn, pawn.Position, reloadableComp, forceReload: false);
			if (foundAmmo.NullOrEmpty())
			{
				return null;
			}

			Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_ConcealedArmament_ReloadAbility);
			job.targetQueueB = [.. foundAmmo.Select((Thing t) => new LocalTargetInfo(t))];

			int jobCount = foundAmmo.Sum((Thing t) => t.stackCount);
			job.count = Math.Min(jobCount, reloadableComp.MaxAmmoNeeded(true));
			job.ability = ability;
			return job;
		}


		private static Ability FindAbilityNeedingReload(Pawn pawn, out AbilityComp_Reloadable reloadableComp)
		{
			foreach (Ability ability in pawn.abilities?.abilities)
			{
				AbilityComp_Reloadable comp = ability.CompOfType<AbilityComp_Reloadable>();
				if (comp != null && comp.NeedsReload(true))
				{
					reloadableComp = comp;
					return ability;
				}
			}
			reloadableComp = null;
			return null;
		}


		private static List<Thing> FindEnoughAmmo(Pawn pawn, IntVec3 rootCell, AbilityComp_Reloadable reloadable, bool forceReload)
		{
			if (reloadable == null)
			{
				return null;
			}
			IntRange desiredQuantity = new(reloadable.MinAmmoNeeded(forceReload), reloadable.MaxAmmoNeeded(forceReload));
			return RefuelWorkGiverUtility.FindEnoughReservableThings(pawn, rootCell, desiredQuantity, (Thing t) => t.def == reloadable.AmmoDef);
		}


		public override float GetPriority(Pawn pawn)
		{
			return 5.9f;
		}
	}
}
