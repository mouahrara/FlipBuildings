using System;
using HarmonyLib;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches.SF
{
	internal class SolidFoundationsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFSolidFoundationsType, "LoadContentPacks", new Type[] { typeof(bool) }),
				postfix: new HarmonyMethod(typeof(BuildingDataUtility), nameof(BuildingDataUtility.LoadContent))
			);
		}
	}
}
