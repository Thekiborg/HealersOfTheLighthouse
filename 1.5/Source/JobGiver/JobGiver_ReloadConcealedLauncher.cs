using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobGiver_ReloadConcealedLauncher : ThinkNode_JobGiver
	{
		AbilityComp_ConcealedLauncher CompCL;
		private readonly IntRange WantedThings = new(1, 1);


		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn is null) return null;

			CompCL = pawn.abilities?.GetAbility(HealersOfTheLighthouse_AbilityDefOfs.HOTL_ConcealedArmament_Launcher)?.CompOfType<AbilityComp_ConcealedLauncher>();

			if (CompCL is null) return null;
			if (CompCL.NumberOfUnloadedSlots <= 0) return null;

			if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				return null;
			}

			List<Thing> foundAmmo = null;
			if (CompCL.FindReloadableSlot())
			{
				foundAmmo = RefuelWorkGiverUtility.FindEnoughReservableThings(pawn,
					pawn.Position,
					WantedThings,
					(Thing t) => t.def == CompCL.magazine[CompCL.MagazineSlotToReload].ThingDef && pawn.CanReserve(t)
					);
			}

			if (foundAmmo.NullOrEmpty()) return null;

			Job job = JobMaker.MakeJob(HealersOfTheLighthouse_JobDefOfs.HOTL_ConcealedArmament_ReloadLauncher, foundAmmo[0]);
			job.count = 1;
			return job;
		}


		public override float GetPriority(Pawn pawn)
		{
			return 5.9f;
		}
	}
}
