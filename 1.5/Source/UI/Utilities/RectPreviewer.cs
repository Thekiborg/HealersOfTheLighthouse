namespace HealersOfTheLighthouse
{
	internal static class RectPreviewer
	{
		public static void Preview(this Rect rectToPreview, int level = 1)
		{
			Color color = level switch
			{
				1 => Color.red,
				2 => Color.green,
				3 => Color.blue,
				_ => Color.white,
			};
			Widgets.DrawBoxSolid(rectToPreview, color);
		}
	}
}
