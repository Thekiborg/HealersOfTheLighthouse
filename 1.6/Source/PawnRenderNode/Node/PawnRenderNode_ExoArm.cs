namespace HealersOfTheLighthouse
{
	public class PawnRenderNode_ExoArm : PawnRenderNode
	{
		public PawnRenderNode_ExoArm(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
		{

		}


		public new PawnRenderNodeProperties_ExoArm Props => (PawnRenderNodeProperties_ExoArm)base.Props;


		public override Graphic GraphicFor(Pawn pawn)
		{
			string text = TexPathFor(pawn);
			if (text.NullOrEmpty())
			{
				return null;
			}
			Shader shader = ShaderFor(pawn);
			if (shader == null)
			{
				return null;
			}
			return GraphicDatabase.Get<Graphic_Multi>(text, shader, Vector2.one, ColorFor(pawn));
		}


		protected override string TexPathFor(Pawn pawn)
		{
			return props.texPath;
		}
	}
}
