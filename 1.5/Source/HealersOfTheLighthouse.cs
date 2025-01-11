global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using RimWorld;
global using Verse;
global using UnityEngine;
global using HarmonyLib;

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
