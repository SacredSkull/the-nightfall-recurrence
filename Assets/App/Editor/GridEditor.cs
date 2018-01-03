using System;
using System.Collections.Generic;
using Level;
using Models;
using Presenters;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Editor {
//	[CustomEditor(typeof(GridPresenter))]
//	public class GridEditor : UnityEditor.Editor {
//		private UnityEditor.Editor _editor;
//		public override void OnInspectorGUI() {
//			serializedObject.Update();
//			GridPresenter gp = (GridPresenter) target;
//
//			gp.SetGridEditor();
//			CreateCachedEditor(gp.gridData, null, ref _editor);
//			
//			_editor?.OnInspectorGUI();
//		}
//	}

	[CustomEditor(typeof(GridPresenter))]
	public class GridEditor : UnityEditor.Editor {
		private UnityEditor.Editor _editor;
		private int counter = 0;
		/*
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			GridPresenter gp = (GridPresenter) target;
			
			if(gp.gridData?.rows == null && counter++ % 2 == 0)
				gp.SetGridEditor();
			
			for (int height = gp.GeometryLayer.Height; height-- > 0;) {
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
				if (gp.gridData.rows.Length <= height) continue;
				for (int width = gp.GeometryLayer.Width; width-- > 0;) {
					if (gp.gridData.rows[height].data.Length <= width) continue;
					string type = gp.gridData.rows[height].data[width];
					GUIStyle style = new GUIStyle {
						stretchWidth = false,
						stretchHeight = false,
					};
					switch (type) {
							case "S":
								style.onNormal.textColor = Color.red;
								style.fontStyle = FontStyle.Bold;
								break;
							case "H":
								style.onNormal.textColor = Color.green;
								style.fontStyle = FontStyle.Bold;
								break;
							case "M":
								style.onNormal.textColor = Color.black;
								break;
							case "0":
								style.onNormal.textColor = Color.black;
								style.fontStyle = FontStyle.Italic;
								break;
							case "P":
								style.onNormal.textColor = Color.yellow;
								style.fontStyle = FontStyle.Bold;
								break;
					}
					EditorGUILayout.TextField("", type, style, GUILayout.MaxWidth(15), GUILayout.MinWidth(15));
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		*/
	}
}