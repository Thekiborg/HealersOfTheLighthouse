namespace HealersOfTheLighthouse
{
	public class AbilityComp_GenericSpew : CompAbilityEffect_FireSpew
	{
		private readonly List<IntVec3> tmpCells = [];


		public new AbilityCompProperties_GenericSpew Props => (AbilityCompProperties_GenericSpew)props;
		private Pawn Caster => parent.pawn;


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			GenExplosion.DoExplosion(target.Cell,
				parent.pawn.MapHeld,
				0f,
				Props.spewDamageDef,
				Caster,
				postExplosionSpawnThingDef: Props.filthDef,
				damAmount: Props.damAmount,
				postExplosionSpawnChance: 1f,
				postExplosionGasType: Props.spewGasType,
				chanceToStartFire: Props.chanceToStartFire,
				doVisualEffects: false,
				propagationSpeed: 0.6f,
				doSoundEffects: false,
				flammabilityChanceCurve: parent.verb.verbProps.flammabilityAttachFireChanceCurve,
				overrideCells: AffectedCells(target));
			base.Apply(target, dest);
		}


		private List<IntVec3> AffectedCells(LocalTargetInfo target)
		{
			tmpCells.Clear();
			Vector3 casterPos = Caster.Position.ToVector3Shifted().Yto0();
			IntVec3 targetCell = target.Cell.ClampInsideMap(Caster.Map);
			if (Caster.Position == targetCell)
			{
				return tmpCells;
			}

			// Get the direction of the target cell from the caster's
			float lengthCasterTarget = (targetCell - Caster.Position).LengthHorizontal;
			float normalizedX = (targetCell.x - Caster.Position.x) / lengthCasterTarget;
			float normalizedZ = (targetCell.z - Caster.Position.z) / lengthCasterTarget;

			// Adds 
			targetCell.x = Mathf.RoundToInt(Caster.Position.x + normalizedX * Props.range);
			targetCell.z = Mathf.RoundToInt(Caster.Position.z + normalizedZ * Props.range);

			float angleBetweenTargetAndPawn = Vector3.SignedAngle(targetCell.ToVector3Shifted().Yto0() - casterPos, Vector3.right, Vector3.up);

			// Get cone boundaries
			float halfAllowedWidth = Props.lineWidthEnd / 2f;
			float hypo = Mathf.Sqrt(Mathf.Pow(lengthCasterTarget, 2f) + Mathf.Pow(halfAllowedWidth, 2f));
			float angleRAD = Mathf.Rad2Deg * Mathf.Asin(halfAllowedWidth / hypo);

			int numCellsInRadius = GenRadial.NumCellsInRadius(Props.range);
			for (int i = 0; i < numCellsInRadius; i++)
			{
				IntVec3 cellAroundPawn = Caster.Position + GenRadial.RadialPattern[i];
				float angleDifference = Mathf.DeltaAngle(Vector3.SignedAngle(cellAroundPawn.ToVector3Shifted().Yto0() - casterPos, Vector3.right, Vector3.up), angleBetweenTargetAndPawn);

				// Checks if a cell is inside the cone boundaries, if it is its added to the list of cells that will be highlighted
				if (CanUseCell(cellAroundPawn) && Mathf.Abs(angleDifference) <= angleRAD)
				{
					tmpCells.Add(cellAroundPawn);
				}
			}

			// Add cells in a direct line between the caster's pos and the target's cell
			List<IntVec3> list = GenSight.BresenhamCellsBetween(Caster.Position, targetCell);
			for (int j = 0; j < list.Count; j++)
			{
				IntVec3 intVec3 = list[j];
				if (!tmpCells.Contains(intVec3) && CanUseCell(intVec3))
				{
					tmpCells.Add(intVec3);
				}
			}
			return tmpCells;
			bool CanUseCell(IntVec3 c)
			{
				if (!c.InBounds(Caster.Map))
				{
					return false;
				}
				if (c == Caster.Position)
				{
					return false;
				}
				if (!Props.canHitFilledCells && c.Filled(Caster.Map))
				{
					return false;
				}
				if (!c.InHorDistOf(Caster.Position, Props.range))
				{
					return false;
				}
				return parent.verb.TryFindShootLineFromTo(parent.pawn.Position, c, out _);
			}
		}
	}
}
