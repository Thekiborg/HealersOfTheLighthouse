using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_FollowFirstPawnAndTheorize : JobDriver
	{
		private Thing chair;
		private const float maxDistanceBetweenChairs = 6f * 6f; // Squared 6 for DistanceToSquared to work.
		private AbilityComp_Theorize compTheorize;


		private AbilityComp_Theorize CompTheorize
		{
			get
			{
				compTheorize ??= pawn.abilities.GetAbility(HOTL_AbilityDefOfs.HOTL_SDBD_AbilityTheorize).CompOfType<AbilityComp_Theorize>();
				return compTheorize;
			}
		}
		private Pawn FirstPawn => job?.GetTarget(TargetIndex.A).Pawn;


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
			findAndReserveChair.defaultCompleteMode = ToilCompleteMode.Instant;
			findAndReserveChair.AddPreInitAction(() =>
			{
				IntVec3 firstChairPos = FirstPawn.CurJob.targetB.Thing.Position;

				chair = GenClosest.ClosestThingReachable(
					firstChairPos,
					pawn.Map,
					ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
					PathEndMode.OnCell,
					TraverseParms.For(pawn),
					maxDistance: 100f,
					validator: (Thing t) => SDBDUtilities.SittableValidator(t, pawn)
											&& WanderUtility.InSameRoom(t.Position, firstChairPos, Map)
											&& t.Position.DistanceToSquared(firstChairPos) <= maxDistanceBetweenChairs);
				// Checking if the distance between the chairs is 6 as that is the distance limit for interactions to work.

				if (chair is null)
				{
					Messages.Message(
						"HOTL_AbilityTheorize_CantDo".Translate(pawn.Named("FIRSTPAWN"), FirstPawn.Named("SECONDPAWN")),
						MessageTypeDefOf.RejectInput,
						false);

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
			pathToChair.FailOn(() => FirstPawn.CurJobDef != HOTL_JobDefOfs.HOTL_GetSecondPawnAndTheorize);
			yield return pathToChair;


			Toil chatWithOther = ToilMaker.MakeToil("chatWithOther");
			chatWithOther.activeSkill = () => SkillDefOf.Intellectual;
			chatWithOther.socialMode = RandomSocialMode.Off;
			chatWithOther.FailOn(() => FirstPawn.CurJobDef != HOTL_JobDefOfs.HOTL_GetSecondPawnAndTheorize);
			chatWithOther.FailOn(() => chair.DestroyedOrNull() || chair.IsBurning());
			chatWithOther.defaultCompleteMode = ToilCompleteMode.Delay;
			chatWithOther.defaultDuration = CompTheorize.Props.theorizeSettings.chatDuration;
			chatWithOther.handlingFacing = true;
			chatWithOther.tickIntervalAction = (int delta) =>
			{
				chatWithOther.actor.rotationTracker.FaceTarget(FirstPawn);
				if (pawn.IsHashIntervalTick(CompTheorize.Props.theorizeSettings.chatBubbleDelay.RandomInRange))
				{
					MoteMaker.MakeSpeechBubble(pawn, TextureLibrary.thinkerIcon);
				}
			};
			yield return chatWithOther;
		}
	}
}
