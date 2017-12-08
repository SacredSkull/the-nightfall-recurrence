using System.Collections.Generic;
using JetBrains.Annotations;
using Karma;
using Karma.Metadata;
using Level;
using Models;
using Presenters;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using Zenject;

namespace Controllers {
	[Controller]
	[UsedImplicitly]
	public class LevelController : IController {
		private GridPresenter _gridPresenter;
		private SelectionPresenter _selectionPresenter;
		private ILayeredGrid<MapItem> _layers;
		
		[Inject]
		public LevelController(ILayeredGrid<MapItem> layers) {
			_layers = layers;
		}

		public void SetupGrid(GridPresenter gridPresenter) {
			_gridPresenter = gridPresenter;
			
			_gridPresenter.GridElementSingleClick += FindRelevantTile;			
			_gridPresenter.SetGrid(
				_layers.GetLayer(LayerNames.GEOMETRY_LAYER), 
				_layers.GetLayer(LayerNames.ENTITY_LAYER), 
				_layers.GetLayer(LayerNames.OVERLAY_LAYER)
			);

			_gridPresenter.Render();
		}

		public void SetupSelection(SelectionPresenter selectionPresenter) {
			_selectionPresenter = selectionPresenter;
		}

		private void FindRelevantTile(TilePresenter tile, GameObject obj) {
			Renderer renderer = tile.GetComponent<Renderer>();
			renderer.enabled = !renderer.enabled; 
			Vector2 mapPos = tile.MapItem.GetPosition();
			KeyValuePair<string, GridPiece<MapItem>> layerItem = _layers.GetHighestElement(mapPos, LayerNames.ENTITY_LAYER);
			_selectionPresenter?.handleSelection(layerItem.Value.Value, layerItem.Key);
		}

		public void OnDestroy() {
			
		}
	}
}