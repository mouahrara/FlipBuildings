﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches.AT
{
	internal class BuildingPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.BuildingPatchType, "DrawPrefix"),
				transpiler: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawPrefixTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> DrawPrefixTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					instanceType: typeof(Building),
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.BuildingPatchType, "DrawPrefix");
		}
	}
}
