using System.Collections.Generic;
using Level;
using Models;
using UnityEditor;
using UnityEngine;
using Utility.Collections.Grid;

namespace Controllers {
	[ExecuteInEditMode]
	public class ActiveLevelEditor : MonoBehaviour {
		public LevelModel level;
		public delegate void LevelLoadedHandler(GridGraph<MapItem> graph, Dictionary<string, Sprite> sprites);
		public event LevelLoadedHandler DataLoaded;

		private void OnEnable() {
			ReadData();
		}

		private void ReadData() {
			level = new LevelModel(new FakeDatabase());

			level.LevelLoaded += () => {
				DataLoaded?.Invoke(level.graph, level.Sprites);
			};
			level.Load();
		}
	}
}