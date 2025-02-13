namespace HealersOfTheLighthouse
{
    [StaticConstructorOnStartup]
    public static class TextureFinder
    {
        public static readonly Texture2D chatBubbleIcon = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
    }
}
