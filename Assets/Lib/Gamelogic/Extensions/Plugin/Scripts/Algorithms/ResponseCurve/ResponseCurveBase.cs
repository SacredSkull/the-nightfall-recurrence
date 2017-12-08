// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// This class is is the base of the that described in AI Programming Wisdom 1, "The Beauty of Response Curves", by Bob Alexander.
	///
	/// This class represents a piecewise linear curve, with input-output pairs at the bends. Outputs can be any type for which 
	/// continuous interpolation mae sense. 
	/// 
	/// The inputs need not be spread uniformly.
	/// </summary>
	/// <typeparam name="T">The number type of the input and output, usually float or double.</typeparam>
	[Version(1, 2)]
	public abstract class ResponseCurveBase<T> // Where T is something that can be interpolated
	{
		#region Private Fields

		private readonly List<T> outputSamples;
		private readonly List<float> inputSamples;

		#endregion

		#region  Properties

		/// <summary>
		/// If the input is below the inputMin given in the constructor, 
		/// the output is clamped to the first output sample.
		/// 
		/// If the input is above the inputMax given in the constructor,
		/// the output is clamped to the last output sample.
		/// 
		/// Otherwise an index is calculated, and the output is interpolated
		/// between outputSample[index] and outputSample[index + 1].
		/// </summary>
		/// <param name="fInput">The input for which output is sought.</param>
		/// <returns></returns>
		public T this[float fInput]
		{
			get
			{
				int index = SearchInput(fInput);

				float inputSampleMin = inputSamples[index];
				float inputSampleMax = inputSamples[index + 1];

				T outputSampleMin = outputSamples[index];
				T outputSampleMax = outputSamples[index + 1];

				return Lerp(fInput, inputSampleMin, inputSampleMax, outputSampleMin, outputSampleMax);
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new ResponseCurveBase.
		/// </summary>
		/// <param name="inputSamples">Samples of input. Assumes input is monotonically increasing.</param>
		/// <param name="outputSamples">Samples of outputs.</param>

		public ResponseCurveBase(IEnumerable<float> inputSamples, IEnumerable<T> outputSamples)
		{
			var minCount = Mathf.Min(inputSamples.Count(), outputSamples.Count());

			if (minCount < 2)
			{
				throw new Exception("There must be at least two samples");
			}

			//TODO:
			//Check that input is monotonous

			this.outputSamples = new List<T>();
			this.outputSamples.AddRange(outputSamples);

			this.inputSamples = new List<float>();
			this.inputSamples.AddRange(inputSamples);

		}

		#endregion

		#region Protected Methods

		protected abstract T Lerp(T outputSampleMin, T outputSampleMax, float t);

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the biggest index i such that <c>mInput[i] &amp;= fInputValue</c>.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private int SearchInput(float input)
		{
			if (input< inputSamples[0])
			{
				return 0;
			}

			if (input > inputSamples[inputSamples.Count - 2])
			{
				return inputSamples.Count - 2; //return the but-last node
			}

			return SearchInput(input, 0, inputSamples.Count - 2);
		}

		private int SearchInput(float input, int start, int end)
		{
			while (true)
			{
				if (end - start <= 1)
				{
					return start;
				}

				int mid = (end - start)/2 + start;
				float midValue = inputSamples[mid];

				if (input.CompareTo(midValue) > 0)
				{
					start = mid;
				}
				else
				{
					end = mid;
				}
			}
		}

		private T Lerp(float input, float inputSampleMin, float inputSampleMax, T outputSampleMin,
			T outputSampleMax)
		{
			if (input <= inputSampleMin)
			{
				return outputSampleMin;
			}

			if (input >= inputSampleMax)
			{
				return outputSampleMax;
			}

			float t = (input - inputSampleMin) / (inputSampleMax - inputSampleMin);

			var output = Lerp(outputSampleMin, outputSampleMax, t);

			return output;
		}

		#endregion
	}
}
