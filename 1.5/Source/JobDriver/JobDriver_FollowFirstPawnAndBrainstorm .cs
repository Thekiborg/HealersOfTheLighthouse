using Verse.AI;

namespace MoyoMedicalExpansion
{
    public class JobDriver_FollowFirstPawnAndTheorize : JobDriver
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

            Toil findAndReserveChair = ToilMaker.MakeToil("findAndReserveChair");
            findAndReserveChair.AddPreInitAction(() =>
            {
                IntVec3 firstPawnChairPos = Target.CurJob.targetB.Thing.Position;
                Log.Message(firstPawnChairPos);

                chair = GenClosest.ClosestThingReachable(firstPawnChairPos, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                PathEndMode.OnCell, TraverseParms.For(pawn), maxDistance: 100f,
                validator: (Thing t) => BaseChairValidator(t)
                            && WanderUtility.InSameRoom(t.Position, firstPawnChairPos, Map));

                if (chair is null)
                {
                    Messages.Message("AbilityTheorize_CantDo".Translate(pawn.Named("PAWN")), MessageTypeDefOf.RejectInput);
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                pawn.Reserve(chair, job);
            });
            yield return findAndReserveChair;


            Toil pathToChair = ToilMaker.MakeToil("pathToChair");
            pathToChair.AddPreInitAction(() =>
            {
                pawn.pather.StartPath(chair, PathEndMode.OnCell);
            });
            pathToChair.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            pathToChair.FailOn(() => Target.CurJobDef != MoyoMedicalExpansion_JobDefOfs.Thek_TakeSecondPawnToTheorize);
            yield return pathToChair;


            Toil chatWithOther = ToilMaker.MakeToil("chatWithOther");
            chatWithOther.activeSkill = () => SkillDefOf.Intellectual;
            chatWithOther.socialMode = RandomSocialMode.Off;
            chatWithOther.FailOn(() => Target.CurJobDef != MoyoMedicalExpansion_JobDefOfs.Thek_TakeSecondPawnToTheorize);
            chatWithOther.FailOn(() => chair.DestroyedOrNull() || chair.IsBurning());
            chatWithOther.defaultCompleteMode = ToilCompleteMode.Delay;
            chatWithOther.defaultDuration = HelperClass_TheorizeAbility.chatDuration;
            chatWithOther.handlingFacing = true;
            chatWithOther.tickAction = () =>
            {
                chatWithOther.actor.rotationTracker.FaceTarget(Target);
                if (pawn.IsHashIntervalTick(HelperClass_TheorizeAbility.chatBubbleDelay.RandomInRange))
                {
                    pawn.skills.Learn(SkillDefOf.Intellectual, 100f);
                    MoteMaker.MakeSpeechBubble(pawn, TextureManager.chatBubbleIcon);
                }
            };
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
