using UnityEngine;

namespace Gamelogic.Extensions.Internal
{
	public static class Utils
	{
		/// <summary>
		/// A palette of default colors.
		/// </summary>
		public static Color[] DefaultColors
		{
			get { return DefaultColorList.Clone() as Color[]; }
		}

		private static readonly Color[] DefaultColorList = new Color[]
		{
			ColorFromInt(133, 219, 233),
			ColorFromInt(198, 224, 34),
			ColorFromInt(255, 215, 87),
			ColorFromInt(228, 120, 129),

			ColorFromInt(42, 192, 217),
			ColorFromInt(114, 197, 29),
			ColorFromInt(247, 188, 0),
			ColorFromInt(215, 55, 82),

			ColorFromInt(205, 240, 246),
			ColorFromInt(229, 242, 154),
			ColorFromInt(255, 241, 153),
			ColorFromInt(240, 182, 187),

			ColorFromInt(235, 249, 252),
			ColorFromInt(241, 249, 204),
			ColorFromInt(255, 252, 193),
			ColorFromInt(247, 222, 217),

			Color.black
		};

		public static readonly Color Red = DefaultColors[7];
		public static readonly Color Yellow = DefaultColors[6];
		public static readonly Color Green = DefaultColors[5];
		public static readonly Color Blue = DefaultColors[4];

		private static Color ColorFromInt(int r, int g, int b)
		{
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
		}
	}
}
