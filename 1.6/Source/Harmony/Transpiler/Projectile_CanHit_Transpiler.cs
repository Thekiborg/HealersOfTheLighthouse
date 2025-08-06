using System.Reflection.Emit;

namespace HealersOfTheLighthouse
{
	/// <summary>
	/// Allows the LightningProjectiles from my lightning abilities to hit back the launcher of the projectile
	/// </summary>
	[HarmonyPatch(typeof(Projectile), "CanHit")]
	internal static class Projectile_CanHit_Transpiler
	{
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
				new(OpCodes.Isinst, typeof(LightningProjectile)),
				new(OpCodes.Brtrue),
			};

			// Looks for the set of instructions inside instructionsToMatch
			// If found, rests at Ldarg.1
			codeMatcher.MatchStartForward(instructionsToMatch);
			codeMatcher.ThrowIfInvalid("Projectile_CanHit_Transpiler couldn't patch it's intended method, find how to report it on Healers of the Lighthouse's steam page.");

			/* The condition before the one we want to skp, if (!thing.Spawned), goes inside the condition we want to skip via jumping
			 * Instead, we move the label that condition jumps to, from the condition we want to skip to the first instruction we're adding
			 * That way, if "if (!thing.Spawned)" returns true, it will jump into our condition.
			 * If our condition is false, it will fall into the "if (thing == launcher)" condition, following the stack without any jumps 
			 */

			// We extract (remove and store?) the label from Ldarg.1 from the instructions we matched
			var ldarg_1_labels = codeMatcher.Instruction.ExtractLabels();

			// And add those extracted labels to the ldarg.0 we are inserting
			instructionsToInsert[0].WithLabels(ldarg_1_labels);

			// We want to jump to the same instruction bne.un.s jumps to
			// As we're resting in Ldarg.1, Bne.un.S is 3 instructions further.
			// Then we assign the third instruction from the ones we want to insert (Brtrue) and we assign it the operand of bne.un.s
			// Should jump to the start of ProjectileHitFlags hitFlags = HitFlags;
			instructionsToInsert[2].operand = codeMatcher.InstructionAt(3).operand;

			codeMatcher.Insert(instructionsToInsert);
			return codeMatcher.InstructionEnumeration();
		}
	}
}
