﻿namespace HealersOfTheLighthouse
{
	public class PawnFlyer_Propelled : PawnFlyer
	{
		private IntVec3 destinationCellInt;
		internal bool targetHitsWall;
		internal DamageInfo collideDamageInfo;


		IntVec3 DestinationCell
		{
			get
			{
				if (destinationCellInt == IntVec3.Zero)
				{
					destinationCellInt = DestinationPos.ToIntVec3();
				}
				return destinationCellInt;
			}
		}


		protected override void TickInterval(int delta)
		{
			base.Tick();

			if (PositionHeld != DestinationCell && MapHeld is not null)
			{
				Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
				fire.fireSize = Fire.MaxFireSize / 3;
				GenSpawn.Spawn(fire, PositionHeld, MapHeld);
			}
		}


		protected override void RespawnPawn()
		{
			if (targetHitsWall)
			{
				FlyingPawn.TakeDamage(collideDamageInfo);
			}

			base.RespawnPawn();
		}
	}
}
