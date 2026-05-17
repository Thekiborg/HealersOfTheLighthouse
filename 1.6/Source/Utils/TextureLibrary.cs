namespace HealersOfTheLighthouse
{
	[StaticConstructorOnStartup]
	internal static class TextureLibrary
	{
		public static readonly Texture2D ThinkerIcon = ContentFinder<Texture2D>.Get("Motes/SDBDThinker");
		public static readonly Texture2D HeartIcon = ContentFinder<Texture2D>.Get("Things/Mote/SpeechSymbols/Romance");
		public static readonly Texture2D CooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));
		public static readonly Texture2D MarbleSingleShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleSingle");
		public static readonly Texture2D MarbleClusterShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleMany");
		public static readonly Texture2D MarbleMedicineShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleHeal");
	}
}
