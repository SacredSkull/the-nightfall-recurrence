using UnityEngine;
using UnityEditor;

namespace Gamelogic.Extensions.Editor
{
	public static class EditorUtils 
	{
		/// <summary>
		/// General utilities used for editor code.
		/// </summary>
		public static void DrawColors(SerializedProperty colorsProp, Rect position)
		{
			int colorCount = colorsProp.arraySize;

			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();

			int columns = (int)(position.width / 16);
			
			float x = position.x;
			float width = 16;
			float height = 16;
			float y = position.y;

			if (columns > 0)
			{
				var indentLevel = EditorGUI.indentLevel;

				EditorGUI.indentLevel = 0;

				for (int i = 0; i < Mathf.Min(100, colorCount); i++)
				{

					if (i != 0 && i % columns == 0)
					{
						x = position.x;
						y += height;
						//EditorGUILayout.EndHorizontal();
						//EditorGUILayout.BeginHorizontal();
					}

					var colorProp = colorsProp.GetArrayElementAtIndex(i);
					var color = colorProp.colorValue;

					//var rect = EditorGUILayout.GetControlRect(false, 16, GUILayout.Width(16));
                    
					//EditorGUI.DrawRect(rect, color);
					EditorGUIUtility.DrawColorSwatch(new Rect(x, y, width - 2, height - 2), color);
					x += width;
				}

				EditorGUI.indentLevel = indentLevel;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

        public static void DrawColors(Color[] colorList, Rect position, int columns = 2, int maxColumns = 100, float widthOffset = -2.0f, float heightOffset = -2.0f)
        {
            var colorCount = colorList.Length;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            var x = position.x;
            var width = position.width / columns;
            var height = position.height;
            var y = position.y;

            if (columns > 0)
            {
                var indentLevel = EditorGUI.indentLevel;
                
                EditorGUI.indentLevel = 0;
                
                for (int i = 0; i < Mathf.Min(maxColumns, colorCount); i++)
                {
                    //Makes a new row
                    if (i != 0 && i % columns == 0)
                    {
                        x = position.x;
                        y += height;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    var color = colorList[i];

                    EditorGUIUtility.DrawColorSwatch(new Rect(x, y, width + widthOffset, height + heightOffset), color);
                    x += width;
                }

                EditorGUI.indentLevel = indentLevel;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        public static void DrawNodeCurve(Rect start, Rect end)
		{
			var endPos = new Vector3(end.x, end.y + end.height / 2, 0);

			DrawNodeCurve(start, endPos);
		}

		public static void DrawNodeCurve(Rect start, Vector3 endPos)
		{
			var startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
			var startTan = startPos + Vector3.right * 50;
			var endTan = endPos + Vector3.left * 50;
			var shadowCol = new Color(0, 0, 0, 0.06f);

			for (int i = 0; i < 3; i++)
			{// Draw a shadow
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
			}

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);

			var oldColor = Handles.color;

			Handles.color = new Color(.5f, 0.1f, 0.1f);

			Handles.DrawSolidDisc(
				(startPos + endPos) / 2,
				Vector3.forward,
				5);

			Handles.color = Color.black;

			Handles.DrawWireDisc(
				(startPos + endPos) / 2,
				Vector3.forward,
				5);

			Handles.color = oldColor;
		}
	}
}