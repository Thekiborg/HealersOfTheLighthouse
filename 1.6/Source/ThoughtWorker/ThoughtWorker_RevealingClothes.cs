namespace HealersOfTheLighthouse
{
    public class ThoughtWorker_RevealingClothes : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            return true;
        }


        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            List<Apparel> wornApparel = p?.apparel.WornApparel;
            foreach (Apparel apparel in wornApparel)
            {
                var modExt = apparel.def.GetModExtension<ModExtension>();
                if (modExt is not null && modExt.isRevealingApparel)
                {
                    return true;
                }
            }
            return ThoughtState.Inactive;
        }
    }
}
