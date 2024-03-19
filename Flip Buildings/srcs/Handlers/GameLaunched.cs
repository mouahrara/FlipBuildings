﻿using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Handlers
{
	internal static class GameLaunchedHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, GameLaunchedEventArgs e)
		{
			BuildingDataHelper.LoadContent();
		}
	}
}
