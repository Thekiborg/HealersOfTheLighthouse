namespace HealersOfTheLighthouse
{
#pragma warning disable IDE0019
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Notify_UsedVerb))]
    internal static class Pawn_DoKillSideEffects_Postfix
    {
        [HarmonyPostfix]
        internal static void AddCheckForPacifistNatureGene(Pawn pawn)
        {
            GeneClass_PacifistNature gene = pawn.genes?.GetGene(HealersOfTheLighthouse_GeneDefOfs.HOTL_PacifistNature) as GeneClass_PacifistNature;

            if (gene is null) return;

            if (AttackingInnocentPawn(pawn))
            {
                gene.GiveThought(HealersOfTheLighthouse_ThoughtDefOfs.HOTL_PacifistHarmedPawn);
            }

        }

        private static bool AttackingInnocentPawn(Pawn thisPawn)
        {
            LocalTargetInfo target;
            if (thisPawn.CurJobDef == JobDefOf.AttackMelee)
            {
                target = thisPawn.CurJob.targetA;
            }
            else
            {
                target = thisPawn.CurrentEffectiveVerb.CurrentTarget;
            }

            Pawn targetPawn = target.Pawn;

            if (targetPawn is not null)
            {
                if (thisPawn.Faction is not null && targetPawn.Faction is not null && targetPawn.Faction.HostileTo(thisPawn.Faction))
                {
                    return false;
                }
                if (targetPawn.HostileTo(thisPawn))
                {
                    return false;
                }
                if (targetPawn.guilt is not null && targetPawn.guilt.IsGuilty)
                {
                    return false;
                }
                if (targetPawn.health.hediffSet.HasHediff(HediffDefOf.Scaria))
                {
                    return false;
                }
                return true;
            }

            return false;
        }
    }
}
