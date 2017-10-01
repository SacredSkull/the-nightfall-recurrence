// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System.Collections.Generic;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// A response curve with outputs of Color.
	/// </summary>
	[Version(1, 2)]
	public class ResponseCurveColor : ResponseCurveBase<Color>
	{
		#region Static Methods

		public static ResponseCurveColor GetLerp(float x0, float x1, Color y0, Color y1)
		{
			var input = new List<float>();
			var output = new List<Color>();

			input.Add(x0);
			input.Add(x1);

			output.Add(y0);
			output.Add(y1);

			var responseCurve = new ResponseCurveColor(input, output);

			return responseCurve;
		}

		#endregion

		#region Constructors

		public ResponseCurveColor(IEnumerable<float> inputSamples, IEnumerable<Color> outputSamples)
			: base(inputSamples, outputSamples)
		{}

		#endregion

		#region Protected Methods

		protected override Color Lerp(Color outputSampleMin, Color outputSampleMax, float t)
		{
			Color output = Color.Lerp(outputSampleMin, outputSampleMax, t);
			return output;
		}

		#endregion
	}
}
