using StardewValley;
using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Hooks
{
	internal static class DayEnding
	{
		/// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, DayEndingEventArgs e)
		{
			if (Game1.IsMasterGame)
				BuildingHelper.Reset();
		}
	}
}
