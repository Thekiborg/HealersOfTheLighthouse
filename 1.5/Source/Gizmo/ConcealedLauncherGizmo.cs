using Verse.Sound;

namespace HealersOfTheLighthouse
{
	[StaticConstructorOnStartup]
	public class ConcealedLauncherGizmo : Gizmo
	{
		/* Fields */
		private const float GizmoHeight = 75f;
		private const float OuterPadding = 5f;
		private const float ShootGizmoSize = GizmoHeight - 2 * OuterPadding;
		private const float MagazineHeight = 40f;
		private const float AmmoSlotPadding = 7f;
		private const float AmmoSlotHeight = MagazineHeight - 2 * AmmoSlotPadding;


		private readonly Ability ability;
		private readonly Pawn caster;
		readonly (ThingDef ThingDef, bool IsLoaded)[] magazine;
		private readonly Texture2D icon;


		private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));
		private readonly SoundDef activateSound = SoundDefOf.Tick_Tiny;


		/* Properties */
		public override bool Disabled
		{ 
			get
			{
				DisabledCheck();
				return disabled;
			}
			set => base.Disabled = value;
		}

		private AbilityComp_ConcealedLauncher CompCL => ability.CompOfType<AbilityComp_ConcealedLauncher>();

		private int NumberOfSlots => CompCL.Props.ammoCapacity;



		public ConcealedLauncherGizmo(Ability ability, Pawn pawn, (ThingDef, bool)[] magazine) : base()
		{
			this.ability = ability;
			caster = pawn;
			icon = ContentFinder<Texture2D>.Get(ability?.def.iconPath);
			this.magazine = magazine;
		}


		/* Methods */
		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect main = new(topLeft.x, topLeft.y, GetWidth(maxWidth), GizmoHeight);
			Widgets.DrawWindowBackground(main);

			Rect shootButtonRect = new(main.xMax - ShootGizmoSize - OuterPadding, main.y + OuterPadding, ShootGizmoSize, ShootGizmoSize);
			DoShootButton(shootButtonRect, parms);


			// OuterPadding * 3 to account for the - 2 * outerPadding from the shoot gizmo size.
			Rect magazineRectBg = new(main.x + OuterPadding,
				main.y + (main.height / 2 - MagazineHeight / 2),
				main.width - ShootGizmoSize - OuterPadding * 3,
				MagazineHeight);
			GenUI.DrawTextureWithMaterial(magazineRectBg, Command.BGTex, null);

			float ammoSlotXPos = magazineRectBg.xMax - AmmoSlotPadding - AmmoSlotHeight;

			DoMagazine(ammoSlotXPos, magazineRectBg);

			return new GizmoResult(GizmoState.Clear);
		}


		private void DoShootButton(Rect buttonRect, GizmoRenderParms parms)
		{
			using (new TextBlock(GameFont.Tiny))
			{
				MouseoverSounds.DoRegion(buttonRect, SoundDefOf.Mouseover_Command);
				Material material = (Disabled || parms.lowLight) ? TexUI.GrayscaleGUI : null;
				using (new TextBlock(parms.lowLight ? Command.LowLightBgColor : Color.white))
				{
					GenUI.DrawTextureWithMaterial(buttonRect, parms.shrunk ? Command.BGTexShrunk : Command.BGTex, material);
					Widgets.DrawTextureFitted(buttonRect, icon, 1f, material);
				}

				if (Mouse.IsOver(buttonRect))
				{
					using TextBlock textBlock = new(GenUI.MouseoverColor);

					// Only shows up when clicking, why?
					if (ability.verb is Verb_CastAbility verb_CastAbility)
					{
						verb_CastAbility.verbProps.DrawRadiusRing_NewTemp(verb_CastAbility.caster.Position, verb_CastAbility);
					}
					ability.OnGizmoUpdate();

					TipSignal tip = ability.def.description;
					if (!disabledReason.NullOrEmpty())
					{
						tip.text += ("\n\n" + "DisabledCommand".Translate() + ": " + disabledReason).Colorize(ColorLibrary.RedReadable);
					}
					TooltipHandler.TipRegion(buttonRect, tip);
				}
				
				if (Widgets.ButtonInvisible(buttonRect))
				{
					if (Disabled)
					{
						if (!disabledReason.NullOrEmpty())
						{
							Messages.Message(disabledReason, MessageTypeDefOf.RejectInput, historical: false);
						}
					}
					else
					{
						activateSound.PlayOneShotOnCamera();
						Find.DesignatorManager.Deselect();
						TargetingParameters targetingParams = new()
						{
							canTargetPawns = true,
							canTargetLocations = true,
							canTargetBuildings = true,
						};
						Find.Targeter.BeginTargeting(ability.verb);
					}
				}

				bool flag = (ability.Casting || KeyBindingDefOf.QueueOrder.IsDownEvent) && !ability.CanQueueCast;
				if (flag)
				{
					Widgets.FillableBar(buttonRect, 0f, cooldownBarTex, null, doBorder: false);
				}
				else if (ability.CooldownTicksRemaining > 0)
				{
					float value = Mathf.InverseLerp(ability.CooldownTicksTotal, 0f, ability.CooldownTicksRemaining);
					Widgets.FillableBar(buttonRect, Mathf.Clamp01(value), cooldownBarTex, null, doBorder: false);
					if (ability.CooldownTicksRemaining > 0)
					{
						string text = ability.CooldownTicksRemaining.ToStringTicksToPeriod();
						Vector2 vector = Text.CalcSize(text);
						vector.x += 2f;
						Rect rect2 = buttonRect;
						rect2.x = buttonRect.x + buttonRect.width / 2f - vector.x / 2f;
						rect2.width = vector.x;
						rect2.height = vector.y;
						Rect position = rect2.ExpandedBy(8f, 0f);
						using TextBlock textBlock = new(TextAnchor.UpperCenter);
						GUI.DrawTexture(position, TexUI.GrayTextBG);
						Widgets.Label(rect2, text);
					}
				}
			}
		}


		private void DoMagazine(float slotXPos, Rect magazineRectBG)
		{
			for (int i = 0; i < NumberOfSlots; i++)
			{
				int index = i;
				var slot = magazine[index];

				Rect ammoSlotRect = new(slotXPos,
					magazineRectBG.y + (magazineRectBG.height / 2 - AmmoSlotHeight / 2),
					AmmoSlotHeight,
					AmmoSlotHeight);


				slotXPos -= AmmoSlotHeight + OuterPadding;


				if (slot.ThingDef is null)
				{
					//GUI.DrawTexture(ammoSlotRect, TexButton.CloseXSmall);
				}
				else
				{
					if (slot.IsLoaded)
					{
						var OptionSelectedBGFillColor = GenColor.FromHex("#ccb664");
						Widgets.DrawBoxSolid(ammoSlotRect, OptionSelectedBGFillColor);
					}
					else
					{
						Widgets.DrawBoxSolid(ammoSlotRect, ColorLibrary.Grey);
					}
					Widgets.DefIcon(ammoSlotRect, slot.ThingDef);
				}

				if (Widgets.ButtonInvisible(ammoSlotRect))
				{
					List<FloatMenuOption> floatMenuOptions = [];

					floatMenuOptions.Add(new FloatMenuOption("Hola".Translate(), () =>
					{
						magazine[index] = new(null, false);
					}));

					foreach (ThingDef ammoDef in CompCL.Props.allowedAmmo)
					{
						floatMenuOptions.Add(new FloatMenuOption(ammoDef.LabelCap, () =>
						{
							magazine[index] = new(ammoDef, true);
						}));
					}
					Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
				}
			}
		}


		public override float GetWidth(float maxWidth)
		{
			// OuterPadding - Ammo selector - OuterPadding - Shoot gizmo - OuterPadding
			// Ammo selector is made of:
			// AmmoSlotPadding - [OuterPadding - AmmoSlotHeight] - AmmoSlotPadding
			// Where [] repeats as many times as AmmoSlots we want, with an OuterPadding between each square (if there's 5 squares, there's 4 spaces between them)
			return OuterPadding * 3 + ShootGizmoSize + AmmoSlotPadding * 2 + NumberOfSlots * AmmoSlotHeight + (OuterPadding * (NumberOfSlots - 1));
		}

		private void DisabledCheck()
		{
			disabled = ability.GizmoDisabled(out var reason);
			if (disabled)
			{
				disabledReason = reason.CapitalizeFirst();
			}
		}
	}
}
