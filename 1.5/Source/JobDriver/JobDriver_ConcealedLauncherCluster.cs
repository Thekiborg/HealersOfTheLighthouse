using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_ConcealedLauncherCluster : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			throw new NotImplementedException();
		}
	}
}
