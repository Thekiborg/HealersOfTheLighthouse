namespace HealersOfTheLighthouse
{
	public class ConcealedLauncherFireMode
	{
		private readonly string label;
		private readonly Texture2D icon;
		private readonly int shotsPerBurst;
		private readonly Action<LocalTargetInfo, LocalTargetInfo> fireModeBehavior;


		public string Label => label;
		public Texture2D Icon => icon;
		public int ShotsPerBurst => shotsPerBurst;
		public Action<LocalTargetInfo, LocalTargetInfo> Shoot => fireModeBehavior;


		public ConcealedLauncherFireMode(string label, Texture2D icon, int shotsPerBurst, Action<LocalTargetInfo, LocalTargetInfo> fireModeBehavior)
		{
			this.label = label;
			this.icon = icon;
			this.shotsPerBurst = shotsPerBurst;
			this.fireModeBehavior = fireModeBehavior;
		}
	}
}
