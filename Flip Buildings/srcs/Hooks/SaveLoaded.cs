using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Hooks
{
	internal static class SaveLoaded
	{
		/// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, SaveLoadedEventArgs e)
		{
			FarmHouseHelper.Load();
		}
	}
}
