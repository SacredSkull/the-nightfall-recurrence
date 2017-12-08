//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Collections;
using UnityEngine;

namespace Gamelogic.Extensions
{
	public class ScreenShotTaker : GLMonoBehaviour
	{
		public KeyCode screenShotKey = KeyCode.Q;
		public int scale;

		public bool automaticScreenShots;
		public float interval = 60;

		public bool clean;

		public GameObject[] dirtyObjects;

		private static ScreenShotTaker instance;


		private static ScreenShotTaker Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<ScreenShotTaker>();
				}

				return instance;
			}
		}

		public void Start()
		{
			if (automaticScreenShots)
			{
				if (clean)
				{
					InvokeRepeating("TakeClean__", interval, interval);
				}
				else
				{
					InvokeRepeating("Take__", interval, interval);
				}
			}
		}

		public void Update()
		{
			if (Input.GetKeyDown(screenShotKey))
			{
				if (clean)
				{
					TakeClean();
				}
				else
				{
					Take();
				}
			}
		}

		public static void Take()
		{
			Instance.Take__();
		}

		public static void TakeClean()
		{
			Instance.TakeClean__();
		}

		private void TakeClean__()
		{
			StartCoroutine(TakeCleanEnumerator());
		}

		private IEnumerator TakeCleanEnumerator()
		{
			foreach (var obj in dirtyObjects)
			{
				obj.SetActive(false);
			}

			yield return new WaitForEndOfFrame();

			Take__();

			yield return new WaitForEndOfFrame();

			foreach (var obj in dirtyObjects)
			{
				obj.SetActive(true);
			}

		}

		private void Take__()
		{
			string path = "screen_" + DateTime.Now.Ticks + ".png";
			ScreenCapture.CaptureScreenshot(path, scale);
		}
	}
}