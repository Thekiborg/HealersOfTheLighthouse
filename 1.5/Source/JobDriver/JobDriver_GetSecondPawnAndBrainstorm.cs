using Verse.AI;

namespace MoyoMedicalExpansion
{
    public class JobDriver_GetSecondPawnAndBrainstorm : JobDriver
    {
        Thing chair;

        Pawn Target => job.GetTarget(TargetIndex.A).Pawn;


        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //this.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            this.FailOnDowned(TargetIndex.A);
            this.FailOnMentalState(TargetIndex.A);
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnInvalidOrDestroyed(TargetIndex.A);


            Toil seekingChairToil = ToilMaker.MakeToil("SeekingChairToil");
            seekingChairToil.defaultCompleteMode = ToilCompleteMode.Instant;
            seekingChairToil.AddPreInitAction(() =>
            {
                chair = GenClosest.ClosestThingReachable(Target.Position, Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                PathEndMode.OnCell, TraverseParms.For(pawn), validator: BaseChairValidator);

                if (chair is null)
                {
                    Messages.Message("Could not do", MessageTypeDefOf.RejectInput);
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }
            });
            yield return seekingChairToil;


            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);


            Toil reserveAndGoToChair = ToilMaker.MakeToil("ReserveAndGoToChair");
            reserveAndGoToChair.AddPreInitAction(() =>
            {
                pawn.Reserve(chair, job);
                pawn.pather.StartPath(chair, PathEndMode.OnCell);
            });
            reserveAndGoToChair.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            reserveAndGoToChair.AddPreInitAction(() =>
            {
                Job followPawn = JobMaker.MakeJob(MoyoMedicalExpansion_JobDefOfs.Thek_FollowFirstPawnToBrainstorm, pawn);
                Target.jobs.jobQueue.EnqueueFirst(followPawn);
                Target.jobs.EndCurrentJob(JobCondition.InterruptForced);
            });
            yield return reserveAndGoToChair;


            Toil chatWithOther = ToilMaker.MakeToil("chatWithOther");
            chatWithOther.activeSkill = () => SkillDefOf.Intellectual;
            chatWithOther.socialMode = RandomSocialMode.Off;
            chatWithOther.FailOn(() => Target.CurJobDef != MoyoMedicalExpansion_JobDefOfs.Thek_FollowFirstPawnToBrainstorm);
            chatWithOther.defaultCompleteMode = ToilCompleteMode.Delay;
            chatWithOther.defaultDuration = HelperClass_BrainstormAbility.chatDuration;
            chatWithOther.handlingFacing = true;
            chatWithOther.tickAction = () =>
            {
                chatWithOther.actor.Rotation = chair.Rotation;
                if (pawn.IsHashIntervalTick(HelperClass_BrainstormAbility.chatBubbleDelay.RandomInRange))
                {
                    // Add interaction thought ONCE
                    // Add intellectual skill exp train
                    MoteMaker.MakeSpeechBubble(pawn, HelperClass_BrainstormAbility.chatBubbleIcon);
                }
            };
            chatWithOther.AddFinishAction(() =>
            {
                ResearchManager researchManager = Find.ResearchManager;
                ResearchProjectDef researchProject = researchManager.GetProject();

                if (researchProject is null)
                {
                    return;
                }
                else
                {
                    int researchPointsToAdd = HelperClass_BrainstormAbility.CalculateResearchPoints(researchProject.baseCost,
                        Target.skills.GetSkill(SkillDefOf.Intellectual).Level);

                    researchManager.AddProgress(researchProject, researchPointsToAdd);
                }
            });
            yield return chatWithOther;
        }


        bool BaseChairValidator(Thing t)
        {
            if (t.def.building == null || !t.def.building.isSittable)
            {
                return false;
            }

            if (!Toils_Ingest.TryFindFreeSittingSpotOnThing(t, pawn, out _))
            {
                return false;
            }
            if (t.IsForbidden(pawn))
            {
                return false;
            }
            if (pawn.IsColonist && t.Position.Fogged(t.Map))
            {
                return false;
            }
            if (!pawn.CanReserve(t))
            {
                return false;
            }
            if (!t.IsSociallyProper(pawn))
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            if (t.HostileTo(pawn))
            {
                return false;
            }
            return true;
        }
    }
}
