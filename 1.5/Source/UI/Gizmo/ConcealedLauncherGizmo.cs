using Verse.Sound;

namespace HealersOfTheLighthouse
{
	public class ConcealedLauncherGizmo : Gizmo
	{
		// --- Fields ---
		private const float OuterPadding = 5f;
		private const float ShootGizmoSize = Height - 2 * OuterPadding;
		private const float MagazineHeight = 40f;
		private const float AmmoSlotPadding = 7f;
		private const float AmmoSlotHeight = MagazineHeight - 2 * AmmoSlotPadding;
		private static readonly Color OptionSelectedBGFillColor = GenColor.FromHex("#ccb664");

		private readonly AbilityComp_ConcealedLauncher CompCL;
		private readonly List<ConcealedLauncherMagazineData> magazine;
		private readonly Texture2D icon;
		
		private readonly SoundDef activateSound = SoundDefOf.Tick_Tiny;


		// --- Properties ---
		private Ability Ability => CompCL.parent;
		private Pawn Caster => Ability.pawn;
		public override bool Disabled
		{
			get
			{
				DisabledCheck();
				return disabled;
			}
			set => base.Disabled = value;
		}
		private int NumberOfSlots => CompCL.Props.ammoCapacity;


		public ConcealedLauncherGizmo(AbilityComp_ConcealedLauncher CompCL, List<ConcealedLauncherMagazineData> magazine) : base()
		{
			this.CompCL = CompCL;
			icon = ContentFinder<Texture2D>.Get(Ability?.def.iconPath);
			this.magazine = magazine;
		}


		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect main = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
			Widgets.DrawWindowBackground(main);

			Rect shootButtonRect = new(main.xMax - ShootGizmoSize - OuterPadding, main.y + OuterPadding, ShootGizmoSize, ShootGizmoSize);
			DoShootButton(shootButtonRect, main.yMax, parms);


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


		private void DoShootButton(Rect buttonRect, float mainYMax, GizmoRenderParms parms)
		{
			using (new TextBlock(GameFont.Tiny))
			{
				// Sound when hovering over the button
				MouseoverSounds.DoRegion(buttonRect, SoundDefOf.Mouseover_Command);
				Material material = (Disabled || parms.lowLight) ? TexUI.GrayscaleGUI : null;

				if (Mouse.IsOver(buttonRect))
				{
					// Draw darkened button background if hovered
					using (new TextBlock(Disabled ? Color.white : GenUI.MouseoverColor))
					{
						GenUI.DrawTextureWithMaterial(buttonRect, parms.shrunk ? Command_Ability.BGTexShrunk : Command_Ability.BGTex, material);
					}

					// Create tooltip
					TipSignal tip = Ability.def.description;
					if (!disabledReason.NullOrEmpty())
					{
						tip.text += ("\n\n" + "DisabledCommand".Translate() + ": " + disabledReason).Colorize(ColorLibrary.RedReadable);
					}
					TooltipHandler.TipRegion(buttonRect, tip);
					Ability.OnGizmoUpdate();
				}
				else
				{
					// Draw regular button background
					using (new TextBlock(parms.lowLight ? Command.LowLightBgColor : Color.white))
					{
						GenUI.DrawTextureWithMaterial(buttonRect, parms.shrunk ? Command_Ability.BGTexShrunk : Command_Ability.BGTex, material);
					}
				}
				// Draw button icon
				Widgets.DrawTextureFitted(buttonRect, CompCL.CurFireMode.Icon, 1f, material);

				// Draw button label within the button's dimensions, but below the main gizmo
				float num = Text.CalcHeight(Ability.def.LabelCap, buttonRect.width + 0.1f);
				Rect rect3 = new(buttonRect.x, mainYMax - num + 12f, buttonRect.width, num);
				// Copied this from vanilla, i have no clue where that 12 comes from.

				// Draw that grey background for labels and the label itself
				GUI.DrawTexture(rect3, TexUI.GrayTextBG);
				using (new TextBlock(TextAnchor.UpperCenter))
				{
					Widgets.Label(rect3, CompCL.CurFireMode.Label.Translate());
				}



				// --- Behavior when the button is clicked ---
				if (Widgets.ButtonInvisible(buttonRect))
				{
					if (Event.current.button == 1)
					{
						activateSound.PlayOneShotOnCamera();
						int fireModeIndex = CompCL.FireModeIndex + 1;
						CompCL.FireModeIndex = fireModeIndex < CompCL.FireModesCount ? fireModeIndex : 0;
					}
					else
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
							Find.Targeter.BeginTargeting(Ability.verb);
						}
					}
				}

				// Resets the cooldown bar
				bool flag = (Ability.Casting || KeyBindingDefOf.QueueOrder.IsDownEvent) && !Ability.CanQueueCast;
				if (flag)
				{
					Widgets.FillableBar(buttonRect, 0f, TextureLibrary.cooldownBarTex, null, doBorder: false);
				}
				// Does the cooldown bar that progressively gets fuller
				else if (Ability.CooldownTicksRemaining > 0)
				{
					float value = Mathf.InverseLerp(Ability.CooldownTicksTotal, 0f, Ability.CooldownTicksRemaining);
					Widgets.FillableBar(buttonRect, Mathf.Clamp01(value), TextureLibrary.cooldownBarTex, null, doBorder: false);
					if (Ability.CooldownTicksRemaining > 0)
					{
						string text = Ability.CooldownTicksRemaining.ToStringTicksToPeriod();
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


				// Draw the ammo icons inside the bar
				if (slot.ThingDef is null)
				{
					GUI.DrawTexture(ammoSlotRect, TexButton.CloseXSmall);
				}
				else
				{
					if (slot.IsLoaded)
					{
						Widgets.DrawBoxSolid(ammoSlotRect, OptionSelectedBGFillColor);
					}
					else
					{
						Widgets.DrawBoxSolid(ammoSlotRect, ColorLibrary.Grey);
					}
					Widgets.DefIcon(ammoSlotRect, slot.ThingDef, scale: 1.8f);

					TooltipHandler.TipRegion(ammoSlotRect, new TipSignal(slot.ThingDef.LabelCap));
				}

				// Float menu to pick the ammo type
				if (Widgets.ButtonInvisible(ammoSlotRect))
				{
					List<FloatMenuOption> floatMenuOptions = [];

					floatMenuOptions.Add(new FloatMenuOption("ConcealedLauncher_EmptySelection".Translate(), () =>
					{
						SpawnLoadedShellIfPresent(slot);
						slot.ThingDef = null;
					}));

					foreach (ThingDef ammoDef in CompCL.Props.allowedAmmo)
					{
						floatMenuOptions.Add(new FloatMenuOption(ammoDef.LabelCap, () =>
						{
							SpawnLoadedShellIfPresent(slot);
							slot.ThingDef = ammoDef;
							slot.IsLoaded = false;
						}));
					}
					Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
				}
			}
		}


		private void SpawnLoadedShellIfPresent(ConcealedLauncherMagazineData slot)
		{
			if (slot.ThingDef is not null && slot.IsLoaded)
			{
				Thing ammo = ThingMaker.MakeThing(slot.ThingDef);
				GenSpawn.Spawn(ammo, Caster.Position, Caster.Map);
			}
		}


		public override float GetWidth(float maxWidth)
		{
			// OuterPadding - Ammo selector - OuterPadding - Shoot gizmo - OuterPadding
			// Ammo selector is made of:
			// AmmoSlotPadding - [OuterPadding - AmmoSlotHeight] - AmmoSlotPadding
			// Where [] repeats as many times as AmmoSlots we want, with an OuterPadding between each square (if there's 5 slots, there's 5 - 1 spaces between them)
			return OuterPadding * 3 + ShootGizmoSize + AmmoSlotPadding * 2 + NumberOfSlots * AmmoSlotHeight + (OuterPadding * (NumberOfSlots - 1));
		}

		private void DisabledCheck()
		{
			disabled = Ability.GizmoDisabled(out var reason);
			if (disabled)
			{
				disabledReason = reason.CapitalizeFirst();
			}
		}
	}
}
