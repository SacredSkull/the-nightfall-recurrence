using Presenters;
using UnityEditor;

namespace Editor {
	[CustomEditor(typeof(TilePresenter))]
	public class MapItemEditor : UnityEditor.Editor {
		private UnityEditor.Editor _editor;
     
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			TilePresenter tile = target as TilePresenter;
			if (tile != null) {
				tile.tileData?.SetData(tile.MapItem);
				
				//var tileData = serializedObject.FindProperty("tileData");
				CreateCachedEditor(tile.tileData, null, ref _editor);
				_editor?.OnInspectorGUI();
			}
			
			
         
			//serializedObject.ApplyModifiedProperties();
		}
	}
}