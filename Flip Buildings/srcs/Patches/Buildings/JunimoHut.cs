﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class JunimoHutPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(JunimoHut), nameof(JunimoHut.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(JunimoHutPatch), nameof(DrawTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					// Flip hut texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Offset bag position
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 48,
					targetInstruction: new(OpCodes.Add),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4, 148),
						new(OpCodes.Sub)
					},
					goNext: false
				),
				new(
					// Flip bag texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 17,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					// Flip light texture
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 17,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(JunimoHut), nameof(JunimoHut.draw));
		}
	}
}
