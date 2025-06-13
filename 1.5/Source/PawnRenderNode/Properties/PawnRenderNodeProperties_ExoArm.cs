namespace HealersOfTheLighthouse
{
#pragma warning disable CA1051
	public class PawnRenderNodeProperties_ExoArm : PawnRenderNodeProperties
	{
		public PawnRenderNodeProperties_ExoArm()
		{
			workerClass = typeof(PawnRenderNodeWorker_ExoArm);
			nodeClass = typeof(PawnRenderNode_ExoArm);
		}

		public float severityToShow;
	}
}
