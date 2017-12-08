using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;

namespace Gamelogic.Extensions.Examples
{
	/// <exclude />
	public static class ExampleGenerators
	{
		//
		public static IGenerator<Vector2> Circle(float radius, int sampleCount)
		{
			var xGenerator = Generator
				.OpenSawTooth(sampleCount)
				.Select(x => radius * Mathf.Cos(2*Mathf.PI*x));

			var yGenerator = Generator
				.OpenSawTooth(sampleCount)
				.Select(x => radius * Mathf.Sin(2 * Mathf.PI * x));

			return Generator.Combine(xGenerator, yGenerator, (x, y) => new Vector2(x, y));
		}

		public static IGenerator<float> BoxBlur(IGenerator<float> generator, int sampleCount)
		{
			return generator
				.Window(sampleCount)
				.Select(window => window.Average());
		}

		public static IGenerator<float> Differentiate(this IGenerator<float> generator)
		{
			return generator
				.Window(2)
				.Select(window => window[1] - window[0]);
		}

		public static IGenerator<float> Differentiate(this IGenerator<float> generator, int n)
		{
			while (true)
			{
				if (n == 0)
				{
					return generator.CloneAndRestart();
				}

				if (n == 1)
				{
					return generator.Differentiate();
				}

				generator = generator.Differentiate();
				n = n - 1;
			}
		}

		public static IGenerator<int> Pattern()
		{
			var zero = Generator.Constant(0);
			var one = Generator.Constant(1);
			var choice = Generator.Count(2);

			var alternate1 = Generator.Choose(new List<IGenerator<int>>{zero, one}, choice);
			var alternate2 = alternate1.RepeatEach(2);

			//will generate 0 0 0 1 1 0 1 1 ...//repeat
			var bin4 = Generator.Choose(new List<IGenerator<int>> {alternate1, alternate2}, choice);

			return bin4;
		}

		public static IGenerator<int> ReallyRandom(int n)
		{
			return Generator.UniformRandomInt(n)
				.Window(2)
				.Where(w => w[0] != w[1])
				.Select(w => w[0])
				.Window(3)
				.Where(w => w[2] - w[1] != w[1] - w[0])
				.Select(w => w[0]);
		}

		public static IGenerator<int> Fibonacci()
		{
			return Generator.Iterate(1, 1, (x, y) => x + y);
		}

		public static IGenerator<float> Convolution(this IGenerator<float> generator, IEnumerable<float> convolutionWindow)
		{
			var windowArray = convolutionWindow.ToArray();

			return generator
				.Window(windowArray.Length)
				.Select(w => w.Select((t, i) => t*windowArray[i]).Sum());
		}

		public static IGenerator<float> Convolution(
			this IGenerator<float> generator, 
			IGenerator<IList<float>> convolutionWindow, 
			int windowLength)
		{
			return Generator.Combine(
				generator.Window(windowLength),
				convolutionWindow,
				(w1, w2) => w1.Select((item, i) => item*w2[i]).Sum());
		}

		private sealed class PaperFoldingGenerator : Generator.AbstractGenerator<int>
		{
			private readonly IGenerator<int> count;
			private readonly Queue<int> queue;
			private int current;

			public override int Current
			{
				get { return current; }
			}

			public PaperFoldingGenerator()
			{
				count = Generator.Count(4);
				queue = new Queue<int>();
			
				MoveNext();
			}

			public override void MoveNext()
			{
				var actionIndex = count.Next();

				switch (actionIndex)
				{
					case 0:
						current = 1;
						break;
					case 1:
						current = queue.Dequeue();
						break;
					case 2:
						current = 0;
						break;
					case 3:
						current = queue.Dequeue();
						break;
				}

				queue.Enqueue(current);
			}

			public override IGenerator<int> CloneAndRestart()
			{
				return new PaperFoldingGenerator();
			}
		}

		public static IGenerator<float> SmoothNoise(int sampleExponent, int smoothSampleCount)
		{
			int sampleCount = 1 << sampleExponent;

			var samples = Generator.UniformRandomFloat().Next(sampleCount);

			return Generator.Repeat(samples)
				.Interpolate(smoothSampleCount, Mathf.Lerp);
		}

		public static IGenerator<float> PerlinNoise(int levels, int sampleExponent)
		{
			int totalSampleCount = 1 << sampleExponent;

			var smoothNoiseLayers = new List<IGenerator<float>>();

			for (int i = 0; i < levels; i++)
			{
				var smoothNoise = SmoothNoise(i, totalSampleCount / (1 << i));
				smoothNoiseLayers.Add(smoothNoise);
			}

			return Generator.Combine(smoothNoiseLayers, layers =>
			{
				float sum = 0f;
				float factorSum = 0f;

				for (int i = 0; i < layers.Count; i++)
				{
					float factor = 1f / (1 << i);

					sum += layers[i] * factor;
					factorSum += factor;
				}

				return sum / factorSum;
			});
		}
	}
}