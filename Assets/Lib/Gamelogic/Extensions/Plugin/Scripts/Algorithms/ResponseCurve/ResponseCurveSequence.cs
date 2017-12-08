// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System.Collections.Generic;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// A response curve where the outputs are sequences of floats.
	/// </summary>
	[Version(1, 4)]
	public class ResponseCurveFloatSequence : ResponseCurveBase<IList<float>>
	{
		#region Constructors

		public ResponseCurveFloatSequence(IEnumerable<float> inputSamples, IEnumerable<IList<float>> outputSamples) 
			: base(inputSamples, outputSamples)
		{
		}

		#endregion

		#region Protected Methods

		protected override IList<float> Lerp(IList<float> outputSampleMin, IList<float> outputSampleMax, float t)
		{
			var output = new List<float>();

			for (int i = 0; i < outputSampleMin.Count; i++)
			{
				output.Add(Mathf.Lerp(outputSampleMin[i], outputSampleMax[i], t));
			}

			return output;
		}

		#endregion
	}
}