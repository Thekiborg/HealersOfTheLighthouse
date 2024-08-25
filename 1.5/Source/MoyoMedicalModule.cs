global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using RimWorld;
global using Verse;
global using UnityEngine;
global using HarmonyLib;

namespace MoyoMedicalExpansion
{
    [StaticConstructorOnStartup]
    internal static class MoyoMedicalExpansion
    {
        static MoyoMedicalExpansion()
        {
            Harmony harmony = new("Thekiborg.MoyoMedicalExpansion");

            harmony.PatchAll();
        }
    }
}
