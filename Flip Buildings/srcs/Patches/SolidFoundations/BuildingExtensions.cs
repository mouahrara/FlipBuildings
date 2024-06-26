﻿using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using FlipBuildings.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace FlipBuildings.Patches.SF
{
	internal class BuildingExtensionsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingExtensionsType, "IsObjectFilteredForChest", new Type[] { typeof(Building), typeof(Item), typeof(Chest), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixBuilding))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingExtensionsType, "IsObjectFilteredForChest", new Type[] { typeof(Building), typeof(Item), typeof(Chest), typeof(bool) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingExtensionsType, "IsAuxiliaryTile", new Type[] { typeof(Building), typeof(Vector2) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixBuilding))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingExtensionsType, "IsAuxiliaryTile", new Type[] { typeof(Building), typeof(Vector2) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingExtensionsType, "ResetLights", new Type[] { typeof(Building), typeof(GameLocation) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixBuilding))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingExtensionsType, "ResetLights", new Type[] { typeof(Building), typeof(GameLocation) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
		}
	}
}
