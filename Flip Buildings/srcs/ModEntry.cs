﻿using System;
using HarmonyLib;
using StardewModdingAPI;
using FlipBuildings.Handlers;
using FlipBuildings.Managers;
using FlipBuildings.Patches;
using FlipBuildings.Utilities;

namespace FlipBuildings
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		// Shared static helpers
		internal static new IModHelper	Helper { get; private set; }
		internal static new IMonitor	Monitor { get; private set; }
		internal static new IManifest	ModManifest { get; private set; }

		public override void Entry(IModHelper helper)
		{
			Helper = base.Helper;
			Monitor = base.Monitor;
			ModManifest = base.ModManifest;

			// Load Mod assets
			AssetManager.Apply();

			// Load Harmony patches
			try
			{
				var harmony = new Harmony(ModManifest.UniqueID);

				// Apply menu patches
				IClickableMenuPatch.Apply(harmony);
				CarpenterMenuPatch.Apply(harmony);

				// Apply building patches
				BuildingPatch.Apply(harmony);
				FishPondPatch.Apply(harmony);
				JunimoHutPatch.Apply(harmony);
				PetBowlPatch.Apply(harmony);

				// Apply location patches
				FarmHousePatch.Apply(harmony);

				// Apply character patches
				NPCPatch.Apply(harmony);

				// Apply AlternativeTextures patches
				if (CompatibilityHelper.IsAlternativeTexturesLoaded)
				{
					Patches.AT.BuildingPatch.Apply(harmony);
				}

				// Apply SolidFoundations patches
				// if (CompatibilityHelper.IsSolidFoundationsLoaded)
				// {
				// 	Patches.SF.GenericBuildingPatch.Apply(harmony);
				// }
			}
			catch (Exception e)
			{
				Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
				return;
			}

			// Subscribe to events
			Helper.Events.GameLoop.GameLaunched += GameLaunchedHandler.Apply;
		}
	}
}
