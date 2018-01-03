using Level;
using Level.Entity;
using Models;
using Presenters;
using RektTransform;
using UnityEngine;
using UnityEngine.UI;
using UnityUtilities.Collections.Grid;

public class DebugGridOverlayPresenter : MVCPresenter2D {
	public GameObject Text;
	public GameObject Container;
	private ILayeredGrid<MapItem> LayeredGrid;
	private bool Active = false;
	private int Scaler = 15;
	private int Counter = 0;

	private void Update() {
		if (Active && Counter++ % 5 == 0) {
			DrawGridOverlay();
		}
	}

	public void SetGrid(ILayeredGrid<MapItem> layeredGrid) {
		LayeredGrid = layeredGrid;
		var rect = Container.GetComponent<RectTransform>();
		rect.SetSize(Scaler * layeredGrid[0].Width, Scaler * layeredGrid[0].Height);
		Active = true;
	}

	public void DrawGridOverlay() {
		foreach(Transform child in Container.transform)
			Destroy(child.gameObject);
		foreach(var result in LayeredGrid.GetLayer(LayerNames.ENTITY_LAYER)){
			var goText = Instantiate(Text, Container.transform).GetComponent<Text>();
			if (result != null) {
				MapItem mi = result;
				if (mi.string_id == "empty") {
					goText.text = "";
				} else if (mi is SoftwareTool) {
					goText.text = result.name.Substring(0, 1);
					goText.fontStyle = FontStyle.BoldAndItalic;
					goText.color = mi is Sentry ? Color.red : Color.green;
				} else {
					goText.text = result.GetType().Name.Substring(0, 1);
					goText.fontStyle = FontStyle.Italic;
				}
			}
		}
	}
}
