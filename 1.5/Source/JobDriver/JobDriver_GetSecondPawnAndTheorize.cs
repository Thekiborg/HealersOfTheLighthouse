using Verse.AI;

namespace MoyoMedicalExpansion
{
    public class JobDriver_GetSecondPawnAndTheorize : JobDriver
    {
        Thing chair;

        Pawn Target => job?.GetTarget(TargetIndex.A).Pawn;


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
            seekingChairToil.AddPreInitAction(() =>
            {
                chair = GenClosest.ClosestThingReachable(Target.Position, Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                PathEndMode.OnCell, TraverseParms.For(pawn), maxDistance: 100f, validator: BaseChairValidator);

                if (chair is null)
                {
                    Messages.Message("AbilityTheorize_CantDo".Translate(pawn.Named("PAWN")), MessageTypeDefOf.RejectInput);
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                job.SetTarget(TargetIndex.B, chair);
            });
            yield return seekingChairToil;


            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);


            Toil reserveAndGiveJob = ToilMaker.MakeToil("reserveAndGiveJob");
            reserveAndGiveJob.AddPreInitAction(() =>
            {
                pawn.Reserve(chair, job);

                Job followPawn = JobMaker.MakeJob(MoyoMedicalExpansion_JobDefOfs.Thek_FollowFirstPawnToTheorize, pawn);
                Target.jobs.jobQueue.EnqueueFirst(followPawn);
                Target.jobs.EndCurrentJob(JobCondition.InterruptForced);
            });
            yield return reserveAndGiveJob;


            Toil pathToChair = ToilMaker.MakeToil("pathToChair");
            pathToChair.AddPreInitAction(() =>
            {
                pawn.pather.StartPath(chair, PathEndMode.OnCell);
            });
            pathToChair.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            pathToChair.FailOn(() => Target.CurJobDef != MoyoMedicalExpansion_JobDefOfs.Thek_FollowFirstPawnToTheorize);
            yield return pathToChair;


            Toil chatWithOther = ToilMaker.MakeToil("chatWithOther");
            chatWithOther.activeSkill = () => SkillDefOf.Intellectual;
            chatWithOther.socialMode = RandomSocialMode.Off;
            chatWithOther.FailOn(() => Target.CurJobDef != MoyoMedicalExpansion_JobDefOfs.Thek_FollowFirstPawnToTheorize);
            chatWithOther.FailOn(() => chair.DestroyedOrNull() || chair.IsBurning());
            chatWithOther.defaultCompleteMode = ToilCompleteMode.Delay;
            chatWithOther.defaultDuration = HelperClass_TheorizeAbility.chatDuration;
            chatWithOther.handlingFacing = true;
            chatWithOther.tickAction = () =>
            {
                chatWithOther.actor.rotationTracker.FaceTarget(Target);
                if (pawn.IsHashIntervalTick(HelperClass_TheorizeAbility.chatBubbleDelay.RandomInRange))
                {
                    pawn.skills.Learn(SkillDefOf.Intellectual, 50f);
                    pawn.interactions.TryInteractWith(Target, MoyoMedicalExpansion_InteractionDefOfs.Thek_InteractionTheorize);
                }
            };
            yield return chatWithOther;


            Toil addProgressToResearchProject = ToilMaker.MakeToil("addProgressToResearchProject");
            addProgressToResearchProject.defaultCompleteMode = ToilCompleteMode.Instant;
            addProgressToResearchProject.AddFinishAction(() =>
            {
                ResearchManager researchManager = Find.ResearchManager;
                ResearchProjectDef researchProject = researchManager.GetProject();

                if (researchProject is null)
                {
                    return;
                }
                else
                {
                    int researchPointsToAdd = HelperClass_TheorizeAbility.CalculateResearchPoints(researchProject.baseCost,
                        Target.skills.GetSkill(SkillDefOf.Intellectual).Level);

                    Messages.Message("AbilityTheorize_Success".Translate(pawn.Named("FIRSTPAWN"), Target.Named("SECONDPAWN"), researchPointsToAdd.Named("POINTS")),
                        MessageTypeDefOf.PositiveEvent);
                    pawn.needs.mood.thoughts.memories.TryGainMemory(MoyoMedicalExpansion_ThoughtDefOfs.Thek_DiscussedWithColonist, Target);

                    researchManager.AddProgress(researchProject, researchPointsToAdd);

                    pawn.abilities.GetAbility(MoyoMedicalExpansion_AbilityDefOfs.Thek_RMBD_AbilityTheorize).CompOfType<AbilityComp_Theorize>().AbilityUsed();
                }
            });
            yield return addProgressToResearchProject;
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
            if (t.GetRoom().ThingCount(t.def) <= 1)
            {
                return false;
            }
            return true;
        }
    }
}
