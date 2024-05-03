using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using FlipBuildings.Utilities;
using StardewValley;

namespace FlipBuildings.Patches.SF
{
	internal class LightManagerPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFLightManagerType, "UpdateLights", new Type[] { typeof(Building), typeof(GameLocation), typeof(GameTime) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixBuilding))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFLightManagerType, "UpdateLights", new Type[] { typeof(Building), typeof(GameLocation), typeof(GameTime) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
		}
	}
}
