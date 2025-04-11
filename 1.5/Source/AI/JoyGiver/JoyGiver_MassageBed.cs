using Verse.AI;
using System.Linq;

namespace HealersOfTheLighthouse
{
	public class JoyGiver_MassageBed : JoyGiver
	{
		MassageSettings ModExt => def.GetModExtension<ModExtension>().massageSettings;
		private readonly List<Thought> bottomThoughtsTemp = [];
		private readonly List<Thought> topThoughtsTemp = [];
		private readonly List<Thought> fusedThoughtsTemp = [];
		private readonly List<Thing> potentialMassageBedTemp = [];


		public override Job TryGiveJob(Pawn pawn)
		{
			Pawn bottom = pawn;
			Pawn top = FindFreeNonRelated(pawn);
			if (top is null)
			{
				return null;
			}


			Thing massageBed = FindMassageBed(bottom, top);
			if (massageBed is null)
			{
				return null;
			}
			if (!JoyGiver_PlayBilliards.ThingHasStandableSpaceOnAllSides(massageBed))
			{
				return null;
			}


			Thing usableBottle = FindOilBottle(bottom, top, massageBed);
			if (usableBottle is null)
			{
				return null;
			}


			Job topJob = JobMaker.MakeJob(HOTL_JobDefOfs.HOTL_TopMassage, bottom, massageBed, usableBottle);
			topJob.count = 1;
			if (!top.jobs.TryTakeOrderedJob(topJob, JobTag.SatisfyingNeeds))
			{
				return null;
			}
			else
			{
				return JobMaker.MakeJob(def.jobDef, top, massageBed);
			}
		}


		private Thing FindMassageBed(Pawn bottom, Pawn top)
		{
			potentialMassageBedTemp.Clear();
			GetSearchSet(bottom, potentialMassageBedTemp);

			if (potentialMassageBedTemp.Count <= 0)
			{
				return null;
			}

			return GenClosest.ClosestThing_Global_Reachable(bottom.Position,
				bottom.Map,
				potentialMassageBedTemp,
				PathEndMode.Touch,
				TraverseParms.For(bottom),
				validator: potentialBed => MassageBedValidator(potentialBed, bottom) && MassageBedValidator(potentialBed, top));
		}


		private bool MassageBedValidator(Thing t, Pawn pawn)
		{
			if (!pawn.CanReserve(t, def.jobDef.joyMaxParticipants))
			{
				return false;
			}
			if (t.IsForbidden(pawn))
			{
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			if (!t.IsSociallyProper(pawn))
			{
				return false;
			}
			if (!t.IsPoliticallyProper(pawn))
			{
				return false;
			}
			if (def.unroofedOnly && t.Position.Roofed(t.Map))
			{
				return false;
			}
			return true;
		}


		private Pawn FindFreeNonRelated(Pawn notThisPawn)
		{

			foreach (Pawn pawn in notThisPawn.Map.mapPawns.FreeAdultColonistsSpawned)
			{
				Log.Message(pawn);

				if (pawn == notThisPawn) continue;
				if (pawn.needs.joy.tolerances.BoredOf(def.joyKind) || !CanBeGivenTo(pawn)) continue;
				if (pawn.Drafted) continue;
				if (!pawn.mindState.IsIdle) continue;
				if (notThisPawn.relations.FamilyByBlood.Contains(pawn)) continue;

				Log.Message(pawn);

				return pawn;
			}
			return null;
		}


		private Thing FindOilBottle(Pawn bottom, Pawn top, Thing massageBed)
		{
			bottom.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(bottomThoughtsTemp);
			top.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(topThoughtsTemp);

			fusedThoughtsTemp.Clear();
			fusedThoughtsTemp.AddRange(bottomThoughtsTemp);
			fusedThoughtsTemp.AddRange(topThoughtsTemp);

			foreach (ThingDef oilDef in ModExt.oils)
			{
				// Check if any of the two pawns has the oil thought
				if (!fusedThoughtsTemp.Any(thought => thought.def == oilDef.GetModExtension<ModExtension>().massageSettings.oilThought))
				{
					Thing oil = GenClosest.ClosestThingReachable(massageBed.Position,
						bottom.Map,
						ThingRequest.ForDef(oilDef),
						PathEndMode.Touch,
						TraverseParms.For(top),
						validator: t => top.CanReserveAndReach(t, PathEndMode.Touch, Danger.None));

					if (oil is null)
					{
						continue;
					}
					else
					{
						return oil;
					}
				}
			}
			return null;
		}
	}
}
