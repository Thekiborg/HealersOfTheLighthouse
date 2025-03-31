namespace HealersOfTheLighthouse
{
    [StaticConstructorOnStartup]
    public static class TextureLibrary
    {
        public static readonly Texture2D chatBubbleIcon = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
		public static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));
		public static readonly Texture2D marbleSingleShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleSingle");
		public static readonly Texture2D marbleClusterShotIcon = ContentFinder<Texture2D>.Get("Abilities/Icons/Icon_MarbleMany");
	}
}
