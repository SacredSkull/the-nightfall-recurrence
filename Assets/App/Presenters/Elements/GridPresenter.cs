using System;
using System.Linq;
using Controllers;
using Installer;
using JetBrains.Annotations;
using Karma;
using Karma.Metadata;
using Level;
using Models;
using Presenters.Layouts;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Utility.Collections.Grid;
using Zenject;
using ILogger = Utility.ILogger;
using Logger = UnityUtilities.Logger;

namespace Presenters {
    [Element(PrefabPath)] [ExecuteInEditMode]
    public class GridPresenter : MVCPresenter {
        public const string PrefabPath = "Grid/GameGrid";
        public GameObject GridContainer;
	    
	    public IGridCollection<MapItem> GeometryLayer;
	    public IGridCollection<MapItem> EntityLayer;
	    public IGridCollection<MapItem> OverlayLayer;

	    private GridPresenter GridPresenterInstance = null;
	    private TilePresenter.Factory TileFactory;
		
	    private GameObject geometryContainer = null;
	    private GameObject entityContainer = null;
	    private GameObject overlayContainer = null;
	    private ILogger Logger = null;
	    
	    public delegate void GridElementSingleClickHandler(TilePresenter tile, GameObject obj);
	    public event GridElementSingleClickHandler GridElementSingleClick;

        //TODO: See https://github.com/cgarciae/karma/issues/1 (the last comment) for layout info
        // Essentially, a layout class needs to be created that stores the rootUI, etc.
	    
	    [Inject] [UsedImplicitly]
	    public void Constructor(ILogger logger, TilePresenter.Factory tileFactory, ILayeredGrid<MapItem> Layers) {
		    Logger = logger;
		    TileFactory = tileFactory;
	    }

	    public void SetGrid(GridCollection<MapItem> geo, GridCollection<MapItem> ent, GridCollection<MapItem> overlay) {
		    GeometryLayer = geo;
		    EntityLayer = ent;
		    OverlayLayer = overlay;
	    }

        public void Render() {
			Logger.Log("Beginning to draw grids...");
			renderGeometryLayer();
			renderEntityLayer();
			createOverlayLayer();

			GeometryLayer.PieceChanged += HandleChangedGridEvent;
			EntityLayer.PieceChanged += HandleChangedGridEvent;
			OverlayLayer.PieceChanged += HandleChangedGridEvent;
		}

	    private void renderGeometryLayer() {
			if (geometryContainer == null) {
				geometryContainer = Instantiate(GridContainer, gameObject.transform.position, Quaternion.identity,
					transform);
			} else {
				foreach (Transform child in geometryContainer.transform) {
					Destroy(child.gameObject);
				}
			}

			geometryContainer.name = "Geometry Grid";
			geometryContainer.GetComponent<GridLayoutGroup>().constraintCount = GeometryLayer.Height;
			geometryContainer.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

			for (int row = 0; row < GeometryLayer.Height; row++) {
				for (int column = 0; column < GeometryLayer.GetRow(row).Count(); column++) {
					GridPiece<MapItem> feature = GeometryLayer.Get(row, column);
					if (feature.Value == null) {
						Logger.Log(string.Format("[GEOMETRY][RASTERING] A Grid Piece has a null value! ID: {0}.",
							feature.ID), LogLevels.ERROR);
					} else {
						//feature.GameObject = Decorate(TileFactory.Create(feature.Value.sprite, feature.Value), feature.Value, "Geometry", geometryContainer).gameObject;
						feature.GameObject = Decorate(TileFactory.Create(), feature.Value, "Geometry", geometryContainer).gameObject;
					}
				}
			}
		}

	    private void renderEntityLayer() {
			if (entityContainer == null) {
				entityContainer = Instantiate(GridContainer, gameObject.transform.position, Quaternion.identity,
					transform);
			} else {
				foreach (Transform child in entityContainer.transform) {
					Destroy(child.gameObject);
				}
			}
			entityContainer.name = "Entity Grid";
			entityContainer.GetComponent<GridLayoutGroup>().constraintCount = EntityLayer.Height;
			entityContainer.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

			for (int row = 0; row < EntityLayer.Height; row++) {
				for (int column = 0; column < EntityLayer.GetRow(row).Count(); column++) {
					GridPiece<MapItem> piece = EntityLayer.Get(row, column);
					

					if (piece.Value != null) {
						if(piece.ID != 0 && piece.Value.sprite == null)
							Logger.Log($"[ENTITY][RASTERING] An entity has no sprite attached! ID: {piece.ID}.", LogLevels.ERROR);
						//piece.GameObject = Decorate(TileFactory.Create(piece.Value.sprite, piece.Value), piece.Value, "Entities", entityContainer).gameObject;
						piece.GameObject = Decorate(TileFactory.Create(), piece.Value, "Entities", entityContainer).gameObject;
					}
				}
			}
		}

	    private void createOverlayLayer() {
			if (overlayContainer == null) {
				overlayContainer = Instantiate(GridContainer, gameObject.transform.position, Quaternion.identity,
					transform);
			} else {
				foreach (Transform child in overlayContainer.transform) {
					Destroy(child.gameObject);
				}
			}
            
			overlayContainer.name = "Overlay Grid";
			overlayContainer.GetComponent<GridLayoutGroup>().constraintCount = OverlayLayer.Height;
			overlayContainer.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

			for (int row = 0; row < OverlayLayer.Height; row++) {
				for (int column = 0; column < OverlayLayer.GetRow(row).Count(); column++) {
					GridPiece<MapItem> piece = OverlayLayer.Get(row, column);
					MapItem blank = MapItem.Factory.CreateBlankTile();
					
					blank.SetPosition(row, column, true, false);

					piece.GameObject = Decorate(TileFactory.Create(), blank, "Overlay", overlayContainer,
						(tile, obj) => {
							GridElementSingleClick?.Invoke(tile, obj);
						}).gameObject;
				}
			}
		}

	    private void HandleChangedGridEvent(object obj) {
			GridCollectionEventArgs<MapItem> conv = obj as GridCollectionEventArgs<MapItem>;
			if(conv != null)
				HandleChangedGridEvent(conv);
		}

	    private void HandleChangedGridEvent(GridCollectionEventArgs<MapItem> args) {
			if (args.GridPiece.GameObject != null) {
				//Logger.Log(string.Format("Event triggered for {0} - its new GO is {1}", args.GridPiece.Position, args.NewGO.name));
				Decorate(args.GridPiece);
			}
		}

	    private TilePresenter Decorate(GridPiece<MapItem> piece, string sortingLayer = null, GameObject parent = null, TilePresenter.SingleClientEventHandler clickHandler = null) {
			return Decorate(piece.GameObject.GetComponent<TilePresenter>(), piece.Value, sortingLayer, parent, clickHandler);
		}

	    private TilePresenter Decorate(TilePresenter tile, MapItem mi, string sortingLayer = null, GameObject parent = null, TilePresenter.SingleClientEventHandler clickHandler = null) {
			tile.MapItem = mi;
		    tile.name = mi.ToString();

		    tile.transform.position = Vector3.zero;
			tile.transform.rotation = Quaternion.identity;
		    
			if (parent != null)
				tile.transform.SetParent(parent.transform, false);
			if (sortingLayer != null && !sortingLayer.Equals(string.Empty))
				tile.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
		    if (clickHandler != null) {
			    tile.SingleClickEvent -= clickHandler;
			    tile.SingleClickEvent += clickHandler;
		    }

		    tile.hideFlags = HideFlags.DontSave;
			return tile;
		}
        
        public override void OnPresenterDestroy() {

        }
    }
}
