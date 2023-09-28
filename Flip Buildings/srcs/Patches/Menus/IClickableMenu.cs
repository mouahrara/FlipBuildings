using HarmonyLib;
using StardewValley.Menus;

namespace FlipBuildings.Patches
{
	internal class IClickableMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
				postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(PopulateClickableComponentListPostfix))
			);
		}

		private static void PopulateClickableComponentListPostfix(IClickableMenu __instance)
		{
			if (__instance.GetType() == typeof(CarpenterMenu))
				__instance.allClickableComponents.Add(CarpenterMenuPatch.flipButton);
		}
	}
}
