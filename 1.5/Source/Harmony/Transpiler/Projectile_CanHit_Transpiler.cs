using System.Reflection.Emit;
using Verse.AI;

namespace HealersOfTheLighthouse
{
	[HarmonyPatch(typeof(Projectile), "CanHit")]
	internal static class Projectile_CanHit_Transpiler
	{
		private static void Test(Projectile proj)
		{
			Log.Message(proj.GetType().FullName);
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> AllowLightningToHitLauncher(IEnumerable<CodeInstruction> codeInstructions)
		{
			CodeMatcher codeMatcher = new(codeInstructions);

			// Set of instructions to be found
			var instructionsToMatch = new CodeMatch[]
			{
				/*
				 * if (thing == launcher)
				 */
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, AccessTools.Field(typeof(Projectile), "launcher")),
				new(OpCodes.Bne_Un_S),
			};

			// Set of instructions to be added
			var instructionsToInsert = new CodeInstruction[]
			{
				/*
				 * if (this is LightningProjectile)
				 */
				new(OpCodes.Ldarg_0),
				new(OpCodes.Call, AccessTools.Method(typeof(Projectile_CanHit_Transpiler), nameof(Test))),
				new(OpCodes.Ldarg_0),
				new(OpCodes.Isinst, typeof(LightningProjectile)),
				new(OpCodes.Brtrue),
			};

			// Looks for the set of instructions inside instructionsToMatch
			// If found, rests at Ldarg.1
			codeMatcher.MatchStartForward(instructionsToMatch);

			// We want to jump to the same instruction bne.un.s jumps to
			// As we're resting in Ldarg.1, Bne.un.S is 3 instructions further.
			// Then we assign the third instruction from the ones we want to insert (Brtrue) and we assign it the operand of bne.un.s
			// Should jump to the start of ProjectileHitFlags hitFlags = HitFlags;
			instructionsToInsert[4].operand = codeMatcher.InstructionAt(3).operand;

			if (codeMatcher.IsInvalid)
			{
				Log.Error("Projectile_CanHit_Transpiler couldn't patch it's intended method, find how to report it on Healers of the Lighthouse's steam page.");
				return codeInstructions;
			}
			else
			{
				codeMatcher.Insert(instructionsToInsert); // Add instructions

				foreach (var instruction in codeMatcher.Instructions())
				{
					Log.Message(instruction.ToString());
				}

				return codeMatcher.InstructionEnumeration(); // Return modified instructions
			}
		}
	}
}
