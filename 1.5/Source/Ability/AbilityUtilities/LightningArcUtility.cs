namespace HealersOfTheLighthouse
{
	public static class LightningArcUtility
	{
		/// <summary>
		/// Gets a list of pawns inside the bounds of a cone<br></br>
		/// The Vector3s must be passed as Vector3Shifted and Yto0.
		/// </summary>
		/// <param name="origin">Origin of the projectile</param>
		/// <param name="dest">Destination of the projectile (what we want to hit)</param>
		/// <param name="map"></param>
		/// <param name="range">Length of the cone</param>
		/// <param name="angleDegs">Angle threshold of the cone</param>
		/// <returns>A list of pawns within the cone</returns>
		public static List<Pawn> GetPawnsForArc(Vector3 origin, Vector3 dest, Map map, float range, int angleDegs)
		{
			List<Pawn> victims = [];

			var maxConeAngle = angleDegs * Mathf.Deg2Rad / 2;

			Vector3 normalizedProjDir = (dest - origin).Yto0().normalized;

			foreach (Pawn pawn in map?.mapPawns.AllPawnsSpawned)
			{
				Vector3 pawnPosVector = pawn.Position.ToVector3Shifted().Yto0();
				Vector3 normalizedPawnDir = (pawnPosVector - dest).Yto0().normalized;
				float distance = Vector3.Distance(dest, pawnPosVector);
				float dotProduct = Vector3.Dot(normalizedPawnDir, normalizedProjDir);

				/*
				Log.Message($"For {pawn}:");
				Log.Message($"Origin {origin} and dest {dest}");
				Log.Message($"MaxConeAngle {maxConeAngle} and proj dir {normalizedProjDir}");
				Log.Message($"pawnPosVector {pawnPosVector}");
				Log.Message($"Normalized pawn dir {normalizedPawnDir}");
				Log.Warning($"Distance {distance} less than {range} ?");
				Log.Warning($"Dot {dotProduct} >= Cosine {Mathf.Cos(maxConeAngle)}?");
				*/

				if (distance <= range && dotProduct >= Mathf.Cos(maxConeAngle))
				{
					victims.Add(pawn);
				}
			}

			return victims;
		}
	}
}
