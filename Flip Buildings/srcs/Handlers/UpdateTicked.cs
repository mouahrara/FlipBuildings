using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Handlers
{
	internal static class UpdateTickedHandler
	{
		internal static bool ReloadContent = false;

		/// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, UpdateTickedEventArgs e)
		{
			if (ReloadContent)
			{
				BuildingDataUtility.LoadContent();
				ReloadContent = false;
			}
		}
	}
}
