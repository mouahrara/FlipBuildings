﻿using System;

namespace FlipBuildings.Utilities
{
	internal class CompatibilityHelper
	{
		internal static readonly bool IsAlternativeTexturesLoaded = ModEntry.Helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures");
		internal static readonly bool IsSolidFoundationsLoaded = ModEntry.Helper.ModRegistry.IsLoaded("PeacefulEnd.SolidFoundations");

		// Get AT types
		internal static readonly Type BuildingPatchType = Type.GetType("AlternativeTextures.Framework.Patches.Buildings.BuildingPatch, AlternativeTextures");

		// Get SF types
		// internal static readonly Type GenericBuildingType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.GenericBuilding, SolidFoundations");
		// internal static readonly Type BuildingDrawLayerType = Type.GetType("SolidFoundations.Framework.Models.Backport.BuildingDrawLayer, SolidFoundations");
		// internal static readonly Type ExtendedBuildingModelType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ExtendedBuildingModel, SolidFoundations");
		// internal static readonly Type ExtendedBuildingDrawLayerType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ExtendedBuildingDrawLayer, SolidFoundations");
		// internal static readonly Type BuildingDataType = Type.GetType("SolidFoundations.Framework.Models.Backport.BuildingData, SolidFoundations");
	}
}
