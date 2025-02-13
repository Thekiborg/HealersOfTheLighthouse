using Verse.AI;

namespace HealersOfTheLighthouse
{
	public class JobDriver_GetSecondPawnAndTheorize : JobDriver
	{
		Thing chair;


		TheorizeAbilitySettings TheorizeSettings => pawn.abilities.GetAbility(HealersOfTheLighthouse_AbilityDefOfs.HOTL_RMBD_AbilityTheorize).CompOfType<AbilityComp_Theorize>().Props.theorizeAbilitySettings;
		Pawn SecondPawn => job?.GetTarget(TargetIndex.A).Pawn;


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
				chair = GenClosest.ClosestThingReachable(SecondPawn.Position,
					Map,
					ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
					PathEndMode.OnCell,
					TraverseParms.For(pawn),
					maxDistance: 100f,
					validator: (Thing t) => TheorizeUtility.SittableValidator(t, pawn));

				if (chair is null)
				{
					Messages.Message(
						"AbilityTheorize_CantDo".Translate(pawn.Named("FIRSTPAWN"), SecondPawn.Named("SECONDPAWN")),
						MessageTypeDefOf.RejectInput,
						false);

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

				Job followPawn = JobMaker.MakeJob(HealersOfTheLighthouse_JobDefOfs.HOTL_FollowFirstPawnToTheorize, pawn);
				SecondPawn.jobs.TryTakeOrderedJob(followPawn);
			});
			yield return reserveAndGiveJob;


			Toil pathToChair = ToilMaker.MakeToil("pathToChair");
			pathToChair.AddPreInitAction(() =>
			{
				pawn.pather.StartPath(chair, PathEndMode.OnCell);
			});
			pathToChair.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			pathToChair.FailOn(() => SecondPawn.CurJobDef != HealersOfTheLighthouse_JobDefOfs.HOTL_FollowFirstPawnToTheorize);
			yield return pathToChair;


			Toil chatWithOther = ToilMaker.MakeToil("chatWithOther");
			chatWithOther.activeSkill = () => SkillDefOf.Intellectual;
			chatWithOther.socialMode = RandomSocialMode.Off;
			chatWithOther.FailOn(() => SecondPawn.CurJobDef != HealersOfTheLighthouse_JobDefOfs.HOTL_FollowFirstPawnToTheorize);
			chatWithOther.FailOn(() => chair.DestroyedOrNull() || chair.IsBurning());
			chatWithOther.defaultCompleteMode = ToilCompleteMode.Delay;
			chatWithOther.defaultDuration = TheorizeSettings.chatDuration;
			chatWithOther.handlingFacing = true;
			chatWithOther.tickAction = () =>
			{
				chatWithOther.actor.rotationTracker.FaceTarget(SecondPawn);
				if (pawn.IsHashIntervalTick(TheorizeSettings.chatBubbleDelay.RandomInRange))
				{
					pawn.interactions.TryInteractWith(SecondPawn, TheorizeSettings.interactionDef);
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
					pawn.abilities.GetAbility(HealersOfTheLighthouse_AbilityDefOfs.HOTL_RMBD_AbilityTheorize).CompOfType<AbilityComp_Theorize>().AbilityUsed();


					int researchPointsToAdd = TheorizeUtility.CalculateResearchPoints(researchProject.baseCost,
						SecondPawn.skills.GetSkill(SkillDefOf.Intellectual).Level,
						TheorizeSettings);
					researchManager.AddProgress(researchProject, researchPointsToAdd);


					Messages.Message("AbilityTheorize_Success".Translate(pawn.Named("FIRSTPAWN"), SecondPawn.Named("SECONDPAWN"), researchPointsToAdd.Named("POINTS")),
						MessageTypeDefOf.PositiveEvent);


					pawn.needs.mood.thoughts.memories.TryGainMemory(HealersOfTheLighthouse_ThoughtDefOfs.HOTL_TheorizedWithColonist, SecondPawn);
				}
			});
			yield return addProgressToResearchProject;
		}
	}
}
