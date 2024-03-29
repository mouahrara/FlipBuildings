﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Mods;

namespace FlipBuildings.Utilities
{
	internal class PatchHelper
	{
		internal class CodeReplacement {
			internal readonly string				modDataKey;
			internal readonly Type					instanceType;
			internal readonly CodeInstruction[]		instanceInstructions;
			internal readonly CodeInstruction[]		referenceInstructions;
			internal readonly byte					offset;
			internal readonly bool					isNegativeOffset;
			internal readonly CodeInstruction		targetInstruction;
			internal readonly bool					checkOperand;
			internal readonly CodeInstruction[]		replacementInstructions;
			internal readonly bool					goNext;
			internal readonly bool					skip;

			public CodeReplacement(string modDataKey = null, Type instanceType = null, CodeInstruction[] instanceInstructions = null, CodeInstruction[] referenceInstructions = null, byte offset = 0, bool isNegativeOffset = true, CodeInstruction targetInstruction = null, bool checkOperand = true, CodeInstruction[] replacementInstructions = null, bool goNext = true, bool skip = false)
			{
				this.modDataKey = modDataKey ?? ModDataKeys.FLIPPED;
				this.instanceType = instanceType;
				this.instanceInstructions = instanceInstructions ?? new CodeInstruction[] { new(OpCodes.Ldarg_0) };
				this.referenceInstructions = referenceInstructions ?? Array.Empty<CodeInstruction>();
				this.offset = offset;
				this.isNegativeOffset = isNegativeOffset;
				this.targetInstruction = targetInstruction ?? new CodeInstruction(OpCodes.Nop);
				this.checkOperand = checkOperand;
				this.replacementInstructions = replacementInstructions ?? Array.Empty<CodeInstruction>();
				this.goNext = goNext;
				this.skip = skip;
			}

			public CodeReplacement(string modDataKey = null, Type instanceType = null, CodeInstruction[] instanceInstructions = null, CodeInstruction referenceInstruction = null, byte offset = 0, bool isNegativeOffset = true, CodeInstruction targetInstruction = null, bool checkOperand = true, CodeInstruction[] replacementInstructions = null, bool goNext = true, bool skip = false): this(modDataKey, instanceType, instanceInstructions, new CodeInstruction[] { referenceInstruction }, offset, isNegativeOffset, targetInstruction, checkOperand, replacementInstructions, goNext, skip)
			{
			}
		}

		internal static IEnumerable<CodeInstruction> ReplaceInstructionsByOffsets(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, CodeReplacement[] CodeReplacements, Type type, string name)
		{
			try
			{
				int n = 0;
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count; i++)
				{
					bool found = true;

					for (int j = 0; j < CodeReplacements[n].referenceInstructions.Length; i++, j++)
					{
						if (i >= list.Count || !list[i].opcode.Equals(CodeReplacements[n].referenceInstructions[j].opcode) || (list[i].operand is not null && !list[i].operand.Equals(CodeReplacements[n].referenceInstructions[j].operand)))
						{
							i -= j;
							found = false;
							break;
						}
					}
					if (!found)
					{
						continue;
					}
					i--;
					if (CodeReplacements[n].skip)
					{
						n++;
						continue;
					}

					int offset = (CodeReplacements[n].isNegativeOffset ? -1 : 1) * CodeReplacements[n].offset;
					int targetIndex = i + offset;

					if (targetIndex >= 0 && targetIndex < list.Count && list[targetIndex].opcode.Equals(CodeReplacements[n].targetInstruction.opcode) && (!CodeReplacements[n].checkOperand || list[targetIndex].operand is null && CodeReplacements[n].targetInstruction.operand is null || list[targetIndex].operand is not null && list[targetIndex].operand.Equals(CodeReplacements[n].targetInstruction.operand)))
					{
						Label[] labels = Enumerable.Range(0, 2).Select(_ => iLGenerator.DefineLabel()).ToArray();
						List<CodeInstruction> codeInstructions = new() { };

						for (int k = 0; k < CodeReplacements[n].instanceInstructions.Length; k++)
						{
							codeInstructions.Add(new(CodeReplacements[n].instanceInstructions[k].opcode, CodeReplacements[n].instanceInstructions[k].operand) { labels = k == 0 ? list[targetIndex].labels : null });
						}

						codeInstructions.Add(new(OpCodes.Call, (CodeReplacements[n].instanceType ?? type).GetProperty("modData").GetGetMethod()));
						codeInstructions.Add(new(OpCodes.Ldstr, CodeReplacements[n].modDataKey));
						codeInstructions.Add(new(OpCodes.Callvirt, typeof(ModDataDictionary).GetMethod(nameof(ModDataDictionary.ContainsKey))));
						codeInstructions.Add(new(OpCodes.Brfalse_S, labels[0]));

						for (int l = 0; l < CodeReplacements[n].replacementInstructions.Length; l++)
						{
							codeInstructions.Add(new(CodeReplacements[n].replacementInstructions[l].opcode, CodeReplacements[n].replacementInstructions[l].operand));
						}

						codeInstructions.Add(new(OpCodes.Br_S, labels[1]));
						codeInstructions.Add(new(list[targetIndex].opcode, list[targetIndex].operand) { labels = { labels[0] } });
						codeInstructions.Add(new(OpCodes.Nop) { labels = { labels[1] } });

						list.InsertRange(targetIndex, codeInstructions);
						i+= codeInstructions.Count;
						list.RemoveAt(i + offset);

						if (!CodeReplacements[n].goNext)
						{
							i-= 2;
						}
						n++;
						if (n == CodeReplacements.Length)
						{
							break;
						}
					}
				}
				ModEntry.Monitor.Log($"{type.Name}.{name}: {n}/{CodeReplacements.Length} patches", LogLevel.Trace);
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {type.Name}.{name}: {e}", LogLevel.Error);
				return instructions;
			}
		}
	}
}
