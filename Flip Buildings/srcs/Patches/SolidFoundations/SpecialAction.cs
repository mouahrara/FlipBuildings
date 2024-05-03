using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using FlipBuildings.Utilities;
using StardewValley;

namespace FlipBuildings.Patches.SF
{
	internal class SpecialActionPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFSpecialActionType, "Trigger", new Type[] { typeof(Farmer), typeof(Building), typeof(Point) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixBuilding))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFSpecialActionType, "Trigger", new Type[] { typeof(Farmer), typeof(Building), typeof(Point) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
		}
	}
}
