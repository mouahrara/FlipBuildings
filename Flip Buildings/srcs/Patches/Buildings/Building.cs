﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Mods;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class BuildingPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.GetData)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(GetDataPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.drawShadow), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int) }),
				transpiler: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawShadowTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.drawBackground), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawBackgroundTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.getPorchStandingSpot)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(GetPorchStandingSpotPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.getMailboxPosition)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(GetMailboxPositionPostfix))
			);
		}

		private static void GetDataPostfix(Building __instance, ref BuildingData __result)
		{
			if (__result is null)
				return;
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;

			__result = BuildingDataUtility.GetFlippedData(__instance);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					// Flip building texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset hourglass position (Gold Clock turned off)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 41,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)68),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)72)
					},
					goNext: false
				),
				new(
					// Flip hourglass texture (Gold Clock turned off)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset hour hand position (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 57,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)92),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)100)
					},
					goNext: false
				),
				new(
					// Reverse hour hand rotation (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul)
					},
					goNext: false
				),
				new(
					// Flip hour hand texture (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset minute hand position (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 61,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)92),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)100)
					},
					goNext: false
				),
				new(
					// Reverse minute hand rotation (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul)
					},
					goNext: false
				),
				new(
					// Flip minute hand texture (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset clock nub position (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 39,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)92),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)100)
					},
					goNext: false
				),
				new(
					// Flip clock nub texture (Gold Clock turned on)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset bubble position (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 40,
					targetInstruction: new(OpCodes.Mul),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Mul),
						new(OpCodes.Ldc_I4_S, (sbyte)16),
						new(OpCodes.Sub)
					},
					goNext: false
				),
				new(
					// Flip bubble texture (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 4,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset item position (Chest)
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 30,
					targetInstruction: new(OpCodes.Ldc_R4, 4f),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_R4, 12f)
					}
				),
				new(
					// Flip texture (drawLayer)
					conditionInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, typeof(Building).GetProperty("modData").GetGetMethod()),
						new(OpCodes.Ldstr, ModDataKeys.FLIPPED_DRAWLAYERS),
						new(OpCodes.Callvirt, typeof(ModDataDictionary).GetMethod(nameof(ModDataDictionary.ContainsKey)))
					},
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Building), nameof(Building.draw));
		}

		private static IEnumerable<CodeInstruction> DrawShadowTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					// Replace left shadow with right shadow
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 16,
					targetInstruction: new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.leftShadow), BindingFlags.Public | BindingFlags.Static)),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.rightShadow), BindingFlags.Public | BindingFlags.Static))
					},
					goNext: false
				),
				new(
					// Flip right shadow texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip middle shadow texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Replace right shadow with left shadow
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 16,
					targetInstruction: new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.rightShadow), BindingFlags.Public | BindingFlags.Static)),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.leftShadow), BindingFlags.Public | BindingFlags.Static))
					},
					goNext: false
				),
				new(
					// Flip left shadow texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Building), nameof(Building.drawShadow));
		}

		private static IEnumerable<CodeInstruction> DrawBackgroundTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					// Flip background texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Building), nameof(Building.drawBackground));
		}

		private static void GetPorchStandingSpotPostfix(Building __instance, ref Point __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;

			if (__instance.isCabin)
			{
				__result = new Point(__instance.tileX.Value + __instance.tilesWide.Value - 1 - 1, __instance.tileY.Value + __instance.tilesHigh.Value - 1);
			}
		}

		private static void GetMailboxPositionPostfix(Building __instance, ref Point __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;

			if (__instance.isCabin)
			{
				__result = new Point(__instance.tileX.Value, __instance.tileY.Value + __instance.tilesHigh.Value - 1);
			}
		}
	}
}
