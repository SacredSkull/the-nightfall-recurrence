using System.Collections.Generic;
using JetBrains.Annotations;
using Karma;
using Karma.Metadata;
using Level;
using Level.Entity;
using Models;
using Presenters;
using UniRx;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Mapping.Pathing;
using Zenject;

namespace Controllers {
	[Controller]
	[UsedImplicitly]
	public class LevelController : IController {
		private GridPresenter _gridPresenter;
		private SelectionPresenter _selectionPresenter;
		private ILayeredGrid<MapItem> _layers;
		public TurnController _turnController;
		protected UnityUtilities.Management.ILogger Logger;
		protected KeyValuePair<string, GridPiece<MapItem>> _selectedPieceLayer;
		protected IGridGraph Graph;

		[Inject]
		public LevelController(UnityUtilities.Management.ILogger logger, ILayeredGrid<MapItem> layers, TurnController turnController, LevelModel lm) {
			Logger = logger;
			_layers = layers;
			_turnController = turnController;
			lm.LoadedEvent += () => { Graph = lm.graph; };
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
			_selectionPresenter.EndTurnButton.OnClickAsObservable().Subscribe(x => {
				_turnController.EndPlayerTurn();
				Logger.Log("Pressed!");
			});
			//_selectionPresenter.EndTurnButton.onClick.AddListener(_turnController.EndPlayerTurn);
		}

		private void FindRelevantTile(TilePresenter tile, GameObject obj) {
			Renderer renderer = tile.GetComponent<Renderer>();
			//renderer.enabled = !renderer.enabled; 
			Vector2 mapPos = tile.MapItem.GetPosition();
			_selectedPieceLayer = _layers.GetHighestElement(mapPos, LayerNames.ENTITY_LAYER);
			_selectionPresenter?.HandleSelection(_selectedPieceLayer.Value.Value, _selectedPieceLayer.Key);	
		}

		public void ParseMovement(KeyCode key) {
			if (_turnController.TurnState != TurnController.TurnStates.PlayerTurn ||
			    _turnController.PlayerState != TurnController.PlayerTurnState.Selected ||
			    !(_selectionPresenter.currentMI is HackTool tool)) return;
			List<Vector2> potential = Graph.Neighbours(tool.GetPosition(),
				Pathing.UnravelPassingFilters(tool.Governor.StandardPassingFilters()),
				Pathing.UnravelBlockingFilters(tool.Governor.StandardBlacklists()));
			Vector2 direction;
			switch (key) {
				case KeyCode.W:
					direction = new Vector2(-1, 0);
					if (potential.Contains(tool.GetPosition() + direction))
						tool.MoveOffset(direction);
					break;
				case KeyCode.A:
					direction = new Vector2(0, -1);
					if (potential.Contains(tool.GetPosition() + direction))
						tool.MoveOffset(direction);
					break;
				case KeyCode.S:
					direction = new Vector2(1, 0);
					if (potential.Contains(tool.GetPosition() + direction))
						tool.MoveOffset(direction);
					break;
				case KeyCode.D:
					direction = new Vector2(0, 1);
					if (potential.Contains(tool.GetPosition() + direction))
						tool.MoveOffset(direction);
					break;
			}
		}

		public void OnDestroy() {
			throw new System.NotImplementedException();
		}
	}
}