using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Hooks
{
	internal static class ModMessageReceived
	{
		/// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID == ModEntry.ModManifest.UniqueID)
			{
				if (e.Type == "InvokeMethod")
				{
					if (e.ReadAs<string>() == "FarmHouseHelper.Flip()")
					{
						FarmHouseHelper.Flip();
					}
				}
			}
		}
	}
}
