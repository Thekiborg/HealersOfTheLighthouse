global using HarmonyLib;
global using RimWorld;
global using System;
global using System.Collections.Generic;
global using UnityEngine;
global using Verse;

namespace HealersOfTheLighthouse
{
	[StaticConstructorOnStartup]
	internal static class HealersOfTheLighthouse
	{
		static HealersOfTheLighthouse()
		{
			Harmony harmony = new("Thekiborg.HealersOfTheLighthouse");

			harmony.PatchAll();
		}
	}
}
