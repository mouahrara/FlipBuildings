using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using FlipBuildings.Managers;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class CarpenterMenuPatch
	{
		private const int												region_flipButton = 110;
		private static readonly PerScreen<ClickableTextureComponent>	flipButton = new(() => null);
		private static readonly PerScreen<bool>							flipping = new(() => false);

		internal static ClickableTextureComponent FlipButton
		{
			get => flipButton.Value;
			set => flipButton.Value = value;
		}

		internal static bool Flipping
		{
			get => flipping.Value;
			set => flipping.Value = value;
		}

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.PropertySetter(typeof(CarpenterMenu), nameof(CarpenterMenu.readOnly)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReadOnlyPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] { typeof(SpriteBatch) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(DrawPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(DrawPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), "resetBounds"),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ResetBoundsPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.performHoverAction), new Type[] { typeof(int), typeof(int) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(PerformHoverActionPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReceiveLeftClickPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenu)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReturnToCarpentryMenuPostfix))
			);
		}

		private static void ReadOnlyPostfix(CarpenterMenu __instance, ref bool value)
		{
			if (value)
			{
				if (Game1.options.SnappyMenus)
				{
					__instance.populateClickableComponentList();
					__instance.snapToDefaultClickableComponent();
				}
			}
		}

		private static bool DrawPrefix(CarpenterMenu __instance, SpriteBatch b)
		{
			if (Game1.IsFading() || __instance.freeze)
			{
				return true;
			}

			if (__instance.onFarm)
			{
				if (Flipping)
				{
					string hoverText = (string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
					string s = ModEntry.Helper.Translation.Get("Carpenter_SelectBuilding_Flip");

					SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
					__instance.cancelButton.draw(b);
					if (__instance.GetChildMenu() == null)
					{
						__instance.drawMouse(b);
						if (hoverText.Length > 0)
						{
							IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont);
						}
					}
					return false;
				}
			}
			return true;
		}

		private static void DrawPostfix(CarpenterMenu __instance, SpriteBatch b)
		{
			if (Game1.IsFading() || __instance.freeze)
			{
				return;
			}

			if (!__instance.onFarm)
			{
				string hoverText = (string)typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

				FlipButton.draw(b);
				__instance.drawMouse(b);
				if (__instance.GetChildMenu() == null)
				{
					__instance.drawMouse(b);
					if (hoverText.Length > 0)
					{
						IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont);
					}
				}
			}
		}

		private static void ResetBoundsPostfix(CarpenterMenu __instance)
		{
			FlipButton = new ClickableTextureComponent("Flip", new Rectangle(__instance.xPositionOnScreen + __instance.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 384 - 22, __instance.yPositionOnScreen + __instance.maxHeightOfBuildingViewer + 64, 64, 64), null, null, AssetManager.flipButton, new Rectangle(0, 0, 16, 16), 4f)
			{
				myID = region_flipButton,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				upNeighborID = CarpenterMenu.region_appearanceButton,
				visible = !__instance.readOnly && (Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && __instance.TargetLocation.buildings.Any(building => BuildingUtility.CanBeFlipped(building))))
			};
			__instance.cancelButton.leftNeighborID = ClickableComponent.SNAP_AUTOMATIC;
		}

		private static bool PerformHoverActionPrefix(CarpenterMenu __instance, int x, int y)
		{
			if (!__instance.onFarm)
			{
				FlipButton.tryHover(x, y);
				if (FlipButton.containsPoint(x, y))
				{
					typeof(CarpenterMenu).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, (string)ModEntry.Helper.Translation.Get("Carpenter_Flip"));
					return false;
				}
			}
			else
			{
				if ((__instance.Action == CarpenterMenu.CarpentryAction.None && !Flipping) || __instance.freeze)
				{
					return false;
				}
				foreach (Building building2 in __instance.TargetLocation.buildings)
				{
					building2.color = Color.White;
				}

				Vector2 tile = new((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
				Building building = __instance.TargetLocation.getBuildingAt(tile) ?? __instance.TargetLocation.getBuildingAt(new(tile.X, tile.Y + 1f)) ?? __instance.TargetLocation.getBuildingAt(new(tile.X, tile.Y + 2f)) ?? __instance.TargetLocation.getBuildingAt(new(tile.X, tile.Y + 3f));
				BuildingData buildingData = building?.GetData();

				if (buildingData != null)
				{
					int num = (buildingData.SourceRect.IsEmpty ? building.texture.Value.Height : building.GetData().SourceRect.Height) * 4 / 64 - building.tilesHigh.Value;

					if (building.tileY.Value - num > tile.Y)
					{
						building = null;
					}
				}
				if (Flipping)
				{
					if (building != null)
					{
						building.color = BuildingUtility.CanBeFlipped(building) ? Color.Lime : Color.Red * 0.8f;
					}
					return false;
				}
			}
			return true;
		}

		private static bool ReceiveLeftClickPrefix(CarpenterMenu __instance, int x, int y)
		{
			if (__instance.freeze)
			{
				return false;
			}
			if (__instance.cancelButton.containsPoint(x, y))
			{
				return true;
			}
			if (!__instance.onFarm)
			{
				if (FlipButton.containsPoint(x, y) && FlipButton.visible)
				{
					Game1.globalFadeToBlack(__instance.setUpForBuildingPlacement);
					Game1.playSound("smallSelect");
					__instance.onFarm = true;
					Flipping = true;
				}
			}
			if (!__instance.onFarm || __instance.freeze || Game1.IsFading())
			{
				return true;
			}
			if (Flipping)
			{
				Vector2 tile = new((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
				Building buildingAt = __instance.TargetLocation.getBuildingAt(tile);

				BuildingUtility.TryToFlip(buildingAt);
				return false;
			}
			return true;
		}

		private static void ReturnToCarpentryMenuPostfix()
		{
			Flipping = false;
		}
	}
}
