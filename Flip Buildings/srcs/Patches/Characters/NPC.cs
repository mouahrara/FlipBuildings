﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Mods;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class NPCPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(NPC), "updateConstructionAnimation"),
				transpiler: new HarmonyMethod(typeof(NPCPatch), nameof(UpdateConstructionAnimationTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> UpdateConstructionAnimationTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					conditionInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldloc_1),
						new(OpCodes.Call, typeof(Farm).GetMethod(nameof(Farm.GetMainFarmHouse))),
						new(OpCodes.Call, typeof(Building).GetProperty("modData").GetGetMethod()),
						new(OpCodes.Ldstr, ModDataKeys.FLIPPED),
						new(OpCodes.Callvirt, typeof(ModDataDictionary).GetMethod(nameof(ModDataDictionary.ContainsKey)))
					},
					referenceInstruction: new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.warpCharacter), new Type[] { typeof(NPC), typeof(string), typeof(Vector2) })),
					offset: 10,
					targetInstruction: new(OpCodes.Ldc_I4_4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_6)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(NPC), "updateConstructionAnimation");
		}
	}
}
