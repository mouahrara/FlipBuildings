using StardewValley;
using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Hooks
{
	internal static class DayStarted
	{
		/// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, DayStartedEventArgs e)
		{
			if (Game1.IsMasterGame)
				BuildingHelper.Update();
		}
	}
}
