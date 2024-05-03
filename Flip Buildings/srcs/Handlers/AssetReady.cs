using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FlipBuildings.Handlers
{
	internal static class AssetReadyHandler
	{
		/// <inheritdoc cref="IContentEvents.AssetReady"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, AssetReadyEventArgs e)
		{
			if (!Context.IsGameLaunched)
				return;

			if (e.Name.IsEquivalentTo("Data/Buildings"))
			{
				UpdateTickedHandler.ReloadContent = true;
			}
		}
	}
}
