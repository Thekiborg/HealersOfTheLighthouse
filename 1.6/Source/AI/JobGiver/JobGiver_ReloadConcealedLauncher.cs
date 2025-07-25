﻿using System.Linq;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobGiver_ReloadConcealedLauncher : ThinkNode_JobGiver
	{
		AbilityComp_ConcealedLauncher CompCL;
		private static readonly IntRange WantedThings = new(1, 1);


		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn is null) return null;

			if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				return null;
			}

			int slotToReload = int.MinValue;
			CompCL = pawn.abilities?.abilities.Where(ab => ab.CompOfType<AbilityComp_ConcealedLauncher>()?.FindEmptySlot(out slotToReload) != false).FirstOrDefault()?.CompOfType<AbilityComp_ConcealedLauncher>();

			if (CompCL is null) return null;

			List<Thing> foundAmmo = null;
			if (slotToReload >= 0)
			{
				foundAmmo = RefuelWorkGiverUtility.FindEnoughReservableThings(pawn,
					pawn.Position,
					WantedThings,
					(Thing t) => t.def == CompCL.Magazine[slotToReload].ThingDef && pawn.CanReserve(t));
			}
			else
			{
				return null;
			}

			if (foundAmmo.NullOrEmpty()) return null;

			Job job = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_ConcealedArmament_ReloadLauncher, foundAmmo[0]);
			job.count = 1;
			return job;
		}


		public override float GetPriority(Pawn pawn)
		{
			return 5.9f;
		}
	}
}
