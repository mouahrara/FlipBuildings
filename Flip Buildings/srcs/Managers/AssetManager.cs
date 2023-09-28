using Microsoft.Xna.Framework.Graphics;

namespace FlipBuildings.Managers
{
	internal static class AssetManager
	{
		public static Texture2D flipButton;

		internal static void Apply()
		{
			flipButton = ModEntry.Helper.ModContent.Load<Texture2D>("assets/flip_button.png");
		}
	}
}
