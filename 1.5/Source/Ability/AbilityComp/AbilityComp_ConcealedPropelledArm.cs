namespace HealersOfTheLighthouse
{
	public class AbilityComp_ConcealedPropelledArm : CompAbilityEffect // Make parent that disables on rain
	{
		// --- Fields ---
		readonly List<IntVec3> cellsToDrawPreview = [];


		// --- Properties ---
		public new AbilityCompProperties_ConcealedPropelledArm Props => (AbilityCompProperties_ConcealedPropelledArm)props;
		private Pawn Caster => parent.pawn;


		public override void DrawEffectPreview(LocalTargetInfo target)
		{
			base.DrawEffectPreview(target);

			if (target.Pawn is Pawn targetPawn)
			{
				IsWallInPath(targetPawn.Position, GetLandPos(targetPawn), out IntVec3 destCell);

				cellsToDrawPreview.Clear();
				cellsToDrawPreview.Add(destCell);
				GenDraw.DrawFieldEdges(cellsToDrawPreview);
			}
		}


		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Thing targetThing = target.Thing;

			Caster.Drawer.Notify_MeleeAttackOn(targetThing);

			Effecter effect = new(Props.ArmEffecter);
			effect.Trigger(Caster, targetThing);
			effect.Cleanup();

			targetThing.TakeDamage(new DamageInfo(
				Props.ArmDamageInfo.damageDef,
				Props.ArmDamageInfo.amount,
				Props.ArmDamageInfo.armorPenetration,
				instigator: Caster,
				spawnFilth: false));

			TryDoKnockback(targetThing);
		}



		private void TryDoKnockback(Thing target)
		{
			if (target is Pawn targetPawn && !targetPawn.Dead)
			{
				bool targetHitsWall = IsWallInPath(targetPawn.Position, GetLandPos(targetPawn), out IntVec3 destCell);

				var flyer = PawnFlyer.MakeFlyer(Props.pawnFlyerDef,
												targetPawn,
												destCell,
												null,
												null,
												true) as PawnFlyer_Propelled;

				if (targetHitsWall)
				{
					flyer.targetHitsWall = targetHitsWall;
					flyer.collideDamageInfo = new(
						Props.collideDamageInfo.damageDef,
						Props.collideDamageInfo.amount,
						Props.collideDamageInfo.armorPenetration);
				}


				GenSpawn.Spawn(flyer, target.Position, Caster.Map);
			}
		}


		/// <summary>
		/// Checks if there's an impassable cell between 2 positions and gives back the last passable cell.
		/// </summary>
		/// <param name="startingCell">First position.</param>
		/// <param name="intendedLandingCell">Second position.</param>
		/// <param name="adjustedCell">The furthest cell, closest to the impassable one.</param>
		/// <returns>True if there's an impassable cell between the 2 positions, otherwise false.</returns>
		private bool IsWallInPath(IntVec3 startingCell, IntVec3 intendedLandingCell, out IntVec3 adjustedCell)
		{
			IntVec3 prevCell = startingCell;
			foreach (IntVec3 cell in GenSight.PointsOnLineOfSight(startingCell, intendedLandingCell))
			{
				if (cell.Impassable(Caster.Map))
				{
					adjustedCell = prevCell;
					return true;
				}

				prevCell = cell;
			}

			adjustedCell = intendedLandingCell;
			return false;
		}


		/// <summary>
		/// Calculates the cell where the target will land after the ability has been used.
		/// </summary>
		/// <param name="target">Target of the ability</param>
		/// <returns>The cell where the target will land.</returns>
		private IntVec3 GetLandPos(Pawn target)
		{
			// Gets and normalizes the direction, which makes the vector not change in length regardless of where the Caster is compared to the target.
			Vector3 direction = (target.Position.ToVector3Shifted() - Caster.Position.ToVector3Shifted()).normalized;

			// The direction will always be 1, rotated around the target, so by multiplying it by the base length it will move it along the direction.
			// Multiplied by the Caster bodysize, making the target be launched further the bigger the Caster it is.
			// Divided by the target bodysize, making them be launched closer the bigger they are
			Vector3 cellOffsetVector = direction * Props.baseKnockbackLength * Caster.BodySize / target.BodySize;

			// This all gets calculated near 0,0. This places the new vector in relation to the target's location.
			return target.Position + cellOffsetVector.ToIntVec3();
		}
	}
}
