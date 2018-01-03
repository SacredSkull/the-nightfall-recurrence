#if NET_4_6
using System.Collections.Generic;
using UnityEngine;

public class NextFrameHelper : MonoBehaviour
{
	private struct Job
	{
		public int frame;
		public System.Action action;
	}

	private static readonly Queue<Job> queue = new Queue<Job>();

	[RuntimeInitializeOnLoadMethod]
	private static void Initialize()
	{
		var go = new GameObject();
		go.hideFlags = HideFlags.HideAndDontSave;
		go.AddComponent<NextFrameHelper>();
		DontDestroyOnLoad(go);
	}

	public static void Enqueue(System.Action action) => queue.Enqueue(new Job { frame = Time.frameCount, action = action });

	private void Update()
	{
		int currentFrame = Time.frameCount;
		while (queue.Count > 0)
		{
			var job = queue.Peek();
			if (job.frame == currentFrame)
			{
				break;
			}
			queue.Dequeue();
			job.action();
		}
	}
}
#endif