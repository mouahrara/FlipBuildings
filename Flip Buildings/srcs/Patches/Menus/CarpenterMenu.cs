using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using FlipBuildings.Managers;
using FlipBuildings.Utilities;
using StardewModdingAPI.Utilities;

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
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.UpdateAppearanceButtonVisibility)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(UpdateAppearanceButtonVisibilityPostfix))
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
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), "resetBounds"),
				prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ResetBoundsPrefix)),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ResetBoundsPostfix))
			);
		}

		private static void ReadOnlyPostfix(ref bool value)
		{
			if (value)
			{
				FlipButton.visible = false;
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

		private static void UpdateAppearanceButtonVisibilityPostfix(CarpenterMenu __instance)
		{
			__instance.backButton.myID = CarpenterMenu.region_backButton;
			__instance.forwardButton.myID = CarpenterMenu.region_forwardButton;
			__instance.appearanceButton.myID = CarpenterMenu.region_appearanceButton;
			FlipButton = new ClickableTextureComponent("Flip", new Rectangle(__instance.xPositionOnScreen + __instance.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 384 - 20, __instance.yPositionOnScreen + __instance.maxHeightOfBuildingViewer + 64, 64, 64), null, null, AssetManager.flipButton, new Rectangle(0, 0, 16, 16), 4f)
			{
				myID = region_flipButton,
				rightNeighborID = CarpenterMenu.region_paintButton,
				leftNeighborID = CarpenterMenu.region_appearanceButton,
				upNeighborID = CarpenterMenu.region_appearanceButton,
				visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && __instance.TargetLocation.buildings.Any(building => BuildingUtility.CanBeFlipped(building)))
			};
			__instance.paintButton.myID = CarpenterMenu.region_paintButton;
			__instance.moveButton.myID = CarpenterMenu.region_moveBuitton;
			__instance.upgradeIcon.myID = CarpenterMenu.region_upgradeIcon;
			__instance.okButton.myID = CarpenterMenu.region_okButton;
			__instance.demolishButton.myID = CarpenterMenu.region_demolishButton;
			__instance.cancelButton.myID = CarpenterMenu.region_cancelButton;

			__instance.backButton.leftNeighborID = -1;
			__instance.backButton.rightNeighborID = __instance.forwardButton.myID;
			__instance.forwardButton.leftNeighborID = __instance.backButton.myID;
			__instance.forwardButton.rightNeighborID = __instance.appearanceButton.myID;
			__instance.appearanceButton.leftNeighborID = __instance.forwardButton.myID;
			__instance.appearanceButton.rightNeighborID = FlipButton.myID;
			FlipButton.leftNeighborID = __instance.appearanceButton.myID;
			FlipButton.rightNeighborID = __instance.paintButton.myID;
			__instance.paintButton.leftNeighborID = FlipButton.myID;
			__instance.paintButton.rightNeighborID = __instance.moveButton.myID;
			__instance.moveButton.leftNeighborID = __instance.paintButton.myID;
			__instance.moveButton.rightNeighborID = __instance.okButton.myID;
			__instance.upgradeIcon.leftNeighborID = __instance.moveButton.myID;
			__instance.upgradeIcon.rightNeighborID = __instance.okButton.myID;
			__instance.okButton.leftNeighborID = __instance.moveButton.myID;
			__instance.okButton.rightNeighborID = __instance.demolishButton.myID;
			__instance.demolishButton.leftNeighborID = __instance.okButton.myID;
			__instance.demolishButton.rightNeighborID = __instance.cancelButton.myID;
			__instance.cancelButton.leftNeighborID = __instance.demolishButton.myID;
			__instance.cancelButton.rightNeighborID = -1;

			if (!__instance.paintButton.visible && !__instance.moveButton.visible && !__instance.okButton.visible && !__instance.demolishButton.visible)
			{
				FlipButton.visible = false;
			}

			if (!__instance.appearanceButton.visible && !FlipButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else if (!FlipButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.appearanceButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.appearanceButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = FlipButton.myID;
				FlipButton.leftNeighborID = __instance.forwardButton.myID;
				FlipButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = FlipButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !FlipButton.visible && !__instance.moveButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.paintButton.myID;
				__instance.paintButton.leftNeighborID = __instance.forwardButton.myID;
				__instance.paintButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = __instance.paintButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !FlipButton.visible && !__instance.paintButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.moveButton.myID;
				__instance.moveButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else if (!__instance.paintButton.visible && !__instance.moveButton.visible)
			{
				FlipButton.rightNeighborID = __instance.okButton.myID;
				__instance.okButton.leftNeighborID = FlipButton.myID;
			}
			else if (!FlipButton.visible && !__instance.paintButton.visible)
			{
				__instance.appearanceButton.rightNeighborID = __instance.moveButton.myID;
				__instance.moveButton.leftNeighborID = __instance.appearanceButton.myID;
			}
			else if (!__instance.appearanceButton.visible && !FlipButton.visible)
			{
				__instance.forwardButton.rightNeighborID = __instance.paintButton.myID;
				__instance.paintButton.leftNeighborID = __instance.forwardButton.myID;
			}
			else
			{
				if (!__instance.moveButton.visible)
				{
					__instance.paintButton.rightNeighborID = __instance.okButton.myID;
					__instance.okButton.leftNeighborID = __instance.paintButton.myID;
				}
				else if (!__instance.paintButton.visible)
				{
					FlipButton.rightNeighborID = __instance.moveButton.myID;
					__instance.moveButton.leftNeighborID = FlipButton.myID;
				}
				if (!FlipButton.visible)
				{
					__instance.appearanceButton.rightNeighborID = __instance.paintButton.myID;
					__instance.paintButton.leftNeighborID = __instance.appearanceButton.myID;
				}
				else if (!__instance.appearanceButton.visible)
				{
					__instance.forwardButton.rightNeighborID = FlipButton.myID;
					FlipButton.leftNeighborID = __instance.forwardButton.myID;
				}
			}
			if (!__instance.demolishButton.visible)
			{
				__instance.okButton.rightNeighborID = __instance.demolishButton.rightNeighborID;
				__instance.cancelButton.leftNeighborID = __instance.demolishButton.leftNeighborID;
			}
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

		private static void ResetBoundsPrefix(CarpenterMenu __instance, ref (bool, bool, bool, bool, bool, bool)? __state)
		{
			if (__instance.upgradeIcon is not null && __instance.demolishButton is not null && __instance.moveButton is not null && __instance.okButton is not null && __instance.paintButton is not null && FlipButton is not null)
			{
				__state = (__instance.upgradeIcon.visible, __instance.demolishButton.visible, __instance.moveButton.visible, __instance.okButton.visible, __instance.paintButton.visible, FlipButton.visible);
			}
		}

		private static void ResetBoundsPostfix(CarpenterMenu __instance, (bool, bool, bool, bool, bool, bool)? __state)
		{
			if (__state.HasValue)
			{
				__instance.upgradeIcon.visible = __state.Value.Item1;
				__instance.demolishButton.visible = __state.Value.Item2;
				__instance.moveButton.visible = __state.Value.Item3;
				__instance.okButton.visible = __state.Value.Item4;
				__instance.paintButton.visible = __state.Value.Item5;
				FlipButton.visible = __state.Value.Item6;
			}
		}
	}
}
