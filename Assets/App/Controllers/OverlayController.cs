using System.Collections.Generic;
using System.Linq;
using Installers;
using Level;
using Models;
using Presenters;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using Zenject;

namespace Controllers {
	public class OverlayController {
		public enum Overlay {
			ATTACK,
			AUGMENT,
			SELECTION,
			MOVEMENT,
			UP_DIRECTIONAL,
			DOWN_DIRECTIONAL,
			LEFT_DIRECTIONAL,
			RIGHT_DIRECTIONAL,
			EMPTY
		}

		protected AssetInstaller.OverlayAssets spriteSettings;
		
		protected GridCollection<MapItem> _overlayGrid;
		protected List<TilePresenter> _currentOverlays = new List<TilePresenter>();
		public TilePresenter.UnhoverEventHandler DefaultUnhoverHandler = (tile, go) => { tile.ForceSprite(); };
		public TilePresenter.HoverEventHandler DefaultHoverHandler;
		
		[Inject]
		public void Inject(LevelModel lm, AssetInstaller.OverlayAssets settings) {
			lm.LoadedEvent += () => { _overlayGrid = lm.LayeredGrid.GetLayer(LayerNames.OVERLAY_LAYER); };
			spriteSettings = settings;

			DefaultHoverHandler = (tile, go) => { 
				tile.ForceSprite(settings.SelectionSprite);
			};
		}

		public void AllHoverHandlers(TilePresenter.HoverEventHandler hoverHandler, TilePresenter.UnhoverEventHandler unhoverHandler) {
			foreach (var gridPiece in _overlayGrid.Values) {
				var tile = gridPiece.GameObject.GetComponent<TilePresenter>();
				tile.ClearHoverHandlers();
				tile.HoverEvent += hoverHandler ?? DefaultHoverHandler;
				tile.UnhoverEvent += unhoverHandler ?? DefaultUnhoverHandler;
			}
		}

		public void AllDefaultHoverHandlers() {
			AllHoverHandlers(DefaultHoverHandler, DefaultUnhoverHandler);
		}

		public void ClearAllOverlays() {
			foreach (var tile in _currentOverlays) {
				tile.ClearSingleClickHandlers();
				tile.ForceSprite(spriteSettings.EmptySprite);
			}
		}

		public void ClearOverlay(Vector2 pos) {
			var tile = _currentOverlays.FirstOrDefault(x => x.MapItem.GetPosition() == pos);
			if (tile != null) {
				tile.ClearSingleClickHandlers();
				tile.ForceSprite(spriteSettings.EmptySprite);
			}
		}
		
		public void AddOverlay(Vector2 position, Sprite sprite = null, TilePresenter.SingleClickEventHandler clickHandler = null) {
			if (sprite == null)
				sprite = spriteSettings.EmptySprite;
			var gridPiece = _overlayGrid.Get(position);
			TilePresenter tile = gridPiece.GameObject.GetComponent<TilePresenter>();
			if (tile != null) {
				tile.ForceSprite(sprite);
				if (clickHandler != null) {
					tile.SingleClickEvent -= clickHandler;
					tile.SingleClickEvent += clickHandler;
				}
			}
			_currentOverlays.Add(tile);
		}
		
		public void AddOverlay(Vector2 position, Overlay overlay = Overlay.EMPTY, TilePresenter.SingleClickEventHandler clickHandler = null) {
			Sprite selected = spriteSettings.SelectionSprite;
			switch (overlay) {
				case Overlay.ATTACK:
					selected = spriteSettings.AttackSprite;
					break;
				case Overlay.AUGMENT:
					selected = spriteSettings.AugmentSprite;
					break;
				case Overlay.SELECTION:
					selected = spriteSettings.SelectionSprite;
					break;
				case Overlay.MOVEMENT:
					selected = spriteSettings.MovementSprite;
					break;
				case Overlay.UP_DIRECTIONAL:
					selected = spriteSettings.UpDirectionalSprite;
					break;
				case Overlay.DOWN_DIRECTIONAL:
					selected = spriteSettings.DownDirectionalSprite;
					break;
				case Overlay.LEFT_DIRECTIONAL:
					selected = spriteSettings.LeftDirectionalSprite;
					break;
				case Overlay.RIGHT_DIRECTIONAL:
					selected = spriteSettings.RightDirectionalSprite;
					break;
				case Overlay.EMPTY:
					selected = spriteSettings.MissingSprite;
					break;
			}

			if (selected == null)
				selected = spriteSettings.MissingSprite ?? spriteSettings.EmptySprite;
			AddOverlay(position, selected, clickHandler);
		}

		public void NewOverlay(Vector2 position, Sprite sprite, TilePresenter.SingleClickEventHandler clickHandler = null) {
			ClearAllOverlays();
			AddOverlay(position, sprite, clickHandler);
		}
		
		public void NewOverlay(Vector2 position, Overlay overlay, TilePresenter.SingleClickEventHandler clickHandler = null) {
			ClearAllOverlays();
			AddOverlay(position, overlay, clickHandler);
		}
	}
}