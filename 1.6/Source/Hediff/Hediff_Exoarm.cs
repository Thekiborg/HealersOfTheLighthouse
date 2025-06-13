namespace HealersOfTheLighthouse
{
	public class Hediff_Exoarm : Hediff_Implant
	{
		private const float SeverityPerStage = 1f;
		public int NumberOfArms => (int)(Severity / SeverityPerStage);


		public override void Notify_SurgicallyRemoved(Pawn surgeon)
		{
			base.Notify_SurgicallyRemoved(surgeon);

			for (int i = 0; i < NumberOfArms; i++)
			{
				Thing thing = ThingMaker.MakeThing(def.spawnThingOnRemoved);
				GenSpawn.Spawn(thing, pawn.Position, pawn.Map);
			}
		}
	}
}
