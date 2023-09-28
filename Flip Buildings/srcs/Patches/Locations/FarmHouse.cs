using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class FarmHousePatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getPorchStandingSpot)),
				postfix: new HarmonyMethod(typeof(FarmHousePatch), nameof(GetPorchStandingSpotPostfix))
			);
		}

		private static void GetPorchStandingSpotPostfix(FarmHouse __instance, ref Point __result)
		{
			Farm farm = Game1.getFarm();
			if (!farm.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			Point mainFarmHouseEntry = farm.GetMainFarmHouseEntry();
			mainFarmHouseEntry.X -= 2;
			__result = mainFarmHouseEntry;
		}
	}
}
