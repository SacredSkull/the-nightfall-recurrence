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
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			GridPresenter gp = (GridPresenter) target;
			
			if(gp.gridData == null || gp.gridData.rows == null)
				gp.SetGridEditor();

			for (int width = 0; width < gp.gridData.rows.Length; width++) {
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
				for (int height = 0; height < gp.gridData.rows[width].data.Length; height++) {
					string type = gp.gridData.rows[width].data[height];
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
							case "P":
								style.onNormal.textColor = Color.yellow;
								style.fontStyle = FontStyle.Bold;
								break;
					}
					EditorGUILayout.TextField("", type, style, GUILayout.MaxWidth(20), GUILayout.MinWidth(20));
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
	
//	[CustomPropertyDrawer(typeof(GridPresenter))]
//	public class GridDrawer : PropertyDrawer {
//		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//			SerializedProperty gridData = property.FindPropertyRelative("gridProperty");
//			
//			EditorGUI.PrefixLabel(position, label);
//			Rect newPosition = position;
//			newPosition.y += 18f;
//			
//			for(int i=0; i < gridData.arraySize; i++)
//			{
//				SerializedProperty row = gridData.GetArrayElementAtIndex(i).FindPropertyRelative("row");
//				newPosition.height = 20;
//
//				if (row.arraySize != 10)
//					row.arraySize = 10;
//
//				newPosition.width = 70;
//
//				for(int j=0; j < row.arraySize; j++)
//				{
//					EditorGUI.PropertyField(newPosition, row.GetArrayElementAtIndex(j), GUIContent.none);
//					newPosition.x += newPosition.width;
//				}
//
//				newPosition.x = position.x;
//				newPosition.y += 20;
//			}
//		}
//	}
}