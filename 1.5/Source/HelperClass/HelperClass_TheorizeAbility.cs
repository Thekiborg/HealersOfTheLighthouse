namespace MoyoMedicalExpansion
{
    internal class HelperClass_TheorizeAbility
    {
        public static readonly IntRange chatBubbleDelay = new(150, 600);
        public const int chatDuration = 10000;

        const float minOutput = 0.2f;
        const float maxOutput = 1.2f;
        const float minDiminishingOutput = 1.5f;
        const float maxDiminishingOutput = 0.65f;


        /// <summary>
        /// <para>
        /// Calculates an amount of research points based on the current ongoing research's baseCost, and the intellectual level of a pawn.
        /// </para>
        /// <para>
        /// The points are calculated by applying HALF a factor between 0.2f and 1.2f (based on intellectual skill) to the baseCost,<br></br>
        /// then applying a balance factor to make the curve steeper on lower intellectual, while making it lower on higher intellectual.
        /// </para>
        /// This is all calculated from level 0 to the pawn's current level, to ensure the research points given are the highest possible from the curve.
        /// </summary>
        /// <param name="researchTotalPoints">The baseCost from the current research project.</param>
        /// <param name="researcherLevel">The intellectual skill level that we want to take into consideration.</param>
        /// <returns>Some amount of research points to be added to the current progress.</returns>
        public static int CalculateResearchPoints(float researchTotalPoints, float researcherLevel)
        {
            int maxResearchPoints = -1;

            for (int skillLvl = 0; skillLvl <= researcherLevel; skillLvl++)
            {
                float normalizedSkillFactor = NormalizeSkillFactor(skillLvl);
                float normalizedBalanceFactor = NormalizedBalanceFactor(skillLvl);

                int calculatedResearchPoints = Mathf.FloorToInt(researchTotalPoints * (normalizedSkillFactor / 2) * normalizedBalanceFactor);

                if (calculatedResearchPoints > maxResearchPoints)
                {
                    maxResearchPoints = calculatedResearchPoints;
                }
                /*else
                {
                    break;
                }*/
            }

            if (DebugSettings.godMode)
            {
                var researchManager = Find.ResearchManager;
                var project = researchManager.GetProject();
                Log.Message($"The research project {project.label} has a base cost of {project.baseCost}");
                Log.Message($"The ability has been used on a pawn with a research level of {researcherLevel}");
                Log.Message($"The ability should add to the project's progress a total of {Mathf.Clamp(maxResearchPoints, 0, researchTotalPoints)} points");
            }

            return (int)Mathf.Clamp(maxResearchPoints, 0, researchTotalPoints);
        }


        /// <summary>
        /// Creates a factor between 0.2f and 1.2f, meant to make lower intellectual pawns give way less points in general than high intellectual pawns
        /// </summary>
        /// <param name="researcherLevel"></param>
        /// <returns></returns>
        private static float NormalizeSkillFactor(float researcherLevel)
        {
            return minOutput + (researcherLevel / SkillRecord.MaxLevel * (maxOutput - minOutput));
        }


        /// <summary>
        /// Creates a factor inbetween 1.5f and 0.65f, meant to make the curve higher when having a low intellectual skill<br></br>
        /// while highly lowering it when having a high intellectual skill.
        /// </summary>
        private static float NormalizedBalanceFactor(float researcherLevel)
        {
            return minDiminishingOutput + (researcherLevel / SkillRecord.MaxLevel * (maxDiminishingOutput - minDiminishingOutput));
        }
    }
}
