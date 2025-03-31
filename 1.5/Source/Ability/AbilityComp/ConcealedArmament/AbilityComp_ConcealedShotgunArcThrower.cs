using System.Linq;

namespace HealersOfTheLighthouse
{
	public class AbilityComp_ConcealedShotgunArcThrower : AbilityComp_ConcealedArcThrower
	{
		Pawn Caster => parent.pawn;
		const float tempvalue = 5f;
		const int anothertempvalue = 5;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			List<Pawn> victims = [];
			foreach (Pawn pawn in Caster.Map.mapPawns.AllPawnsSpawned)
			{
				if (GenSight.LineOfSightToThing(target.Cell, pawn, Caster.Map)
					&& pawn.Position.DistanceTo(target.Cell) <= tempvalue
					&& !pawn.DeadOrDowned)
				{
					victims.Add(pawn);
				}
			}
			victims.Remove(Caster);
			victims = [.. victims.OrderBy(p => p.Position.DistanceToSquared(Caster.Position))];

			int index = 0;
			for (int i = 0; i < anothertempvalue; i++)
			{
				if (index > victims.Count - 1) index = 0;

				base.Apply(victims[index], dest);
				Log.Message(victims[index]);

				index++;
			}
		}

		public override void DrawEffectPreview(LocalTargetInfo target)
		{
			List<IntVec3> cells = GenRadial.RadialCellsAround(target.Cell, tempvalue, true).ToList();
			GenDraw.DrawFieldEdges(cells);
			base.DrawEffectPreview(target);
		}
	}
}
