namespace HealersOfTheLighthouse
{
	[StaticConstructorOnStartup]
	internal static class TextureLibrary
	{
		public static readonly Texture2D thinkerIcon = ContentFinder<Texture2D>.Get("Motes/SDBDThinker");
		public static readonly Texture2D heartIcon = ContentFinder<Texture2D>.Get("Things/Mote/SpeechSymbols/Romance");
		public static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));
		public static readonly Texture2D marbleSingleShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleSingle");
		public static readonly Texture2D marbleClusterShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleMany");
		public static readonly Texture2D marbleMedicineShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleHeal");
	}
}
