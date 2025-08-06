namespace HealersOfTheLighthouse
{
	public class DesiredHemogenGizmo : Gizmo
	{
		private const float Width = 155f;
		private const float Padding = 5f;
		private const float ButtonHeight = (Height - Padding * 3) / 2;
		private const float ButtonWidth = ButtonHeight + 10f;
		private const float CounterRectWidth = Width - 3 * Padding - ButtonWidth;

		private readonly MapComponent_HOTL mapComp;

		public DesiredHemogenGizmo(MapComponent_HOTL mapComp)
		{
			this.mapComp = mapComp;
		}


		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			if (mapComp is null)
			{
				return new GizmoResult(GizmoState.Clear);
			}
			Rect main = new(topLeft.x, topLeft.y, Width, Height);
			Widgets.DrawWindowBackground(main);

			Rect addButton = new(main.xMax - ButtonWidth - Padding, main.y + Padding, ButtonWidth, ButtonHeight);
			if (Widgets.ButtonImage(addButton, TexButton.ReorderUp))
			{
				GameComponent_HOTL.WantedHemogenCount++;
			}

			Rect substractButton = new(addButton.x, addButton.yMax + Padding, ButtonWidth, ButtonHeight);
			if (Widgets.ButtonImage(substractButton, TexButton.ReorderDown))
			{
				GameComponent_HOTL.WantedHemogenCount--;
			}

			Rect counterRect = new(addButton.x - Padding - CounterRectWidth, addButton.y, CounterRectWidth, Height - 2 * Padding);
			using (new TextBlock(GameFont.Medium, TextAnchor.MiddleCenter))
			{
				Widgets.Label(counterRect, $"{mapComp.TrackedHemogen.Count}/{GameComponent_HOTL.WantedHemogenCount}");
			}
			Rect infoRect = new(counterRect.x, counterRect.y, counterRect.width, counterRect.height);
			using (new TextBlock(GameFont.Tiny, TextAnchor.LowerCenter))
			{
				Widgets.Label(infoRect, "DesiredHemogenGizmo_TextUnderCounter".Translate());
			}

			return new GizmoResult(GizmoState.Clear);
		}


		public override float GetWidth(float maxWidth)
		{
			return Width;
		}
	}
}
