namespace HealersOfTheLighthouse
{
    [StaticConstructorOnStartup]
    public static class TextureManager
    {
        public static readonly Texture2D chatBubbleIcon = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
    }
}
