using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Mods;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class FarmPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Farm), nameof(Farm.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(FarmPatch), nameof(DrawTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Label[] labels = Enumerable.Range(0, 8).Select(_ => iLGenerator.DefineLabel()).ToArray();
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					conditionInstructions: new CodeInstruction[]
					{
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Brfalse_S, labels[0]),
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Callvirt, typeof(Building).GetProperty(nameof(Building.isCabin)).GetGetMethod()),
						new(OpCodes.Brfalse_S, labels[0]),
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Callvirt, typeof(Building).GetProperty(nameof(Building.modData)).GetGetMethod()),
						new(OpCodes.Ldstr, ModDataKeys.FLIPPED),
						new(OpCodes.Callvirt, typeof(ModDataDictionary).GetMethod(nameof(ModDataDictionary.ContainsKey))),
						new(OpCodes.Br_S, labels[1]),
						new(OpCodes.Ldc_I4_0) { labels = { labels[0] } },
						new(OpCodes.Nop) { labels = { labels[1] } },
					},
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 31,
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
					conditionInstructions: new CodeInstruction[]
					{
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Brfalse_S, labels[2]),
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Call, typeof(Building).GetProperty(nameof(Building.modData)).GetGetMethod()),
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Callvirt, typeof(Building).GetProperty(nameof(Building.isCabin)).GetGetMethod()),
						new(OpCodes.Brfalse_S, labels[4]),
						new(OpCodes.Ldstr, ModDataKeys.FLIPPED),
						new(OpCodes.Br_S, labels[5]),
						new(OpCodes.Ldstr, ModDataKeys.FLIPPED_DRAWLAYERS) { labels = { labels[4] } },
						new(OpCodes.Callvirt, typeof(ModDataDictionary).GetMethod(nameof(ModDataDictionary.ContainsKey))) { labels = { labels[5] } },
						new(OpCodes.Br_S, labels[3]),
						new(OpCodes.Ldc_I4_0) { labels = { labels[2] } },
						new(OpCodes.Nop) { labels = { labels[3] } }
					},
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 4,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					conditionInstructions: new CodeInstruction[]
					{
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Brfalse_S, labels[6]),
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Callvirt, typeof(Building).GetProperty(nameof(Building.isCabin)).GetGetMethod()),
						new(OpCodes.Brfalse_S, labels[6]),
						new(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod()),
						new(OpCodes.Call, typeof(Utility).GetMethod(nameof(Utility.getHomeOfFarmer), new Type[] { typeof(Farmer) })),
						new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.ParentBuilding))),
						new(OpCodes.Callvirt, typeof(Building).GetProperty(nameof(Building.modData)).GetGetMethod()),
						new(OpCodes.Ldstr, ModDataKeys.FLIPPED),
						new(OpCodes.Callvirt, typeof(ModDataDictionary).GetMethod(nameof(ModDataDictionary.ContainsKey))),
						new(OpCodes.Br_S, labels[7]),
						new(OpCodes.Ldc_I4_0) { labels = { labels[6] } },
						new(OpCodes.Nop) { labels = { labels[7] } },
					},
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 33,
					targetInstruction: new(OpCodes.Add),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_S, (sbyte)16),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Farm), nameof(Farm.draw));
		}
	}
}
