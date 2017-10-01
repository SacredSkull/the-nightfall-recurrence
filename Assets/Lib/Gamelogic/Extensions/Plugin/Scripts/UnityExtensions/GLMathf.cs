// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Methods for additional math functions.
	/// </summary>
	[Version(1, 4)]
	public static class GLMathf 
	{
		#region Constants

		public static readonly float Sqrt3 = Mathf.Sqrt(3);

		#endregion

		#region Static Methods

		public static float Wlerp01(float v1, float v2, float t)
		{
			GLDebug.Assert(InRange(v1, 0, 1), "v1 is not in [0, 1)");
			GLDebug.Assert(InRange(v2, 0, 1), "v2 is not in [0, 1)");

			if (Mathf.Abs(v1 - v2) <= 0.5f)
			{
				return Mathf.Lerp(v1, v2, t);
			}
			else if (v1 <= v2)
			{
				return Wrap01(Mathf.Lerp(v1 + 1, v2, t));
			}
			else
			{
				return Wrap01(Mathf.Lerp(v1, v2 + 1, t));
			}
		}

		public static bool InRange01(float value)
		{
			return InRange(value, 0, 1);
		}

		public static bool InRange(float value, float closedLeft, float openRight)
		{
			return value >= closedLeft && value < openRight;
		}

		public static float Wrap01(float value)
		{
			int n = Mathf.FloorToInt(value);
			float result = value - n;

			GLDebug.Assert(InRange01(result), "result is not in [0, 1)");
			return result;
		}

		/// <summary>
		/// Returns the highest integer equal to the given float.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <returns>System.Int32.</returns> 
		//TODO: Make this function obsolete and use Unity's instead.
		public static int FloorToInt(float x)
		{
			var n = (int)x; //truncate

			if (n > x)
			{
				n = n - 1;
			}

			return n;
		}

		/// <summary>
		/// Mod operator that also works for negative m.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <param name="n">The n.</param>
		/// <returns>System.Int32.</returns>
		[Version(2, 2)]
		public static int FloorMod(int m, int n)
		{
			if (m >= 0)
			{
				return m % n;
			}

			return (m - 2 * m * n) % n;
		}

		/// <summary>
		/// Mod operator that also works for negative m.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <param name="n">The n.</param>
		/// <returns>System.Int32.</returns>
		[Version(2, 2)]
		public static float FloorMod(float m, float n)
		{
			if (m >= 0)
			{
				return m % n;
			}

			return (m % n) + n;
		}



		/// <summary>
		/// Floor division that also work for negative m.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <param name="n">The n.</param>
		/// <returns>System.Int32.</returns>
		[Version(2, 2)]
		public static int FloorDiv(int m, int n)
		{
			if (m >= 0)
			{
				return m / n;
			}

			int t = m / n;

			if (t * n == m)
			{
				return t;
			}

			return t - 1;
		}

		public static float Frac(float x)
		{
			return x - FloorToInt(x);
		}

		public static int Sign(float p)
		{
			if (p > 0) return 1;
			if (p < 0) return -1;

			return 0;
		}

		public static int Sign(int p)
		{
			if (p > 0) return 1;
			if (p < 0) return -1;

			return 0;
		}
		#endregion

		#region Obsolete
		[Obsolete("Use FloorDiv instead")]
		public static int Div(int m, int n)
		{
			return FloorDiv(m, n);
		}

		[Obsolete("Use FloorMod instead")]
		public static int Mod(int m, int n)
		{
			return FloorMod(m, n);
		}

		[Obsolete("Use FloorMod instead")]
		public static float Mod(float m, float n)
		{
			return FloorMod(m, n);
		}
		#endregion
	}
}