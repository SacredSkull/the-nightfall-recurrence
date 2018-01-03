using System.Linq;
using JetBrains.Annotations;
using Karma;
using Karma.Metadata;
using Level;
using Level.Entity;
using UnityEngine;
using UnityEngine.UI;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Management;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Presenters {
    [Element(PrefabPath)] [ExecuteInEditMode]
    public class GridPresenter : MVCPresenter {
        public const string PrefabPath = "Grid/GameGrid";
        public GameObject GridContainer;	    
	    public IGridCollection<MapItem> GeometryLayer;
	    public IGridCollection<MapItem> EntityLayer;
	    public IGridCollection<MapItem> OverlayLayer;
	    public ILayeredGrid<MapItem> LayeredGrid;

	    private GridPresenter GridPresenterInstance = null;
	    private TilePresenter.Factory TileFactory;
		
	    private GameObject geometryContainer = null;
	    private GameObject entityContainer = null;
	    private GameObject overlayContainer = null;
	    private ILogger Logger = null;
	    private MapItemFactory _miFactory;
	    
	    public delegate void GridElementSingleClickHandler(TilePresenter tile, GameObject obj);
	    public event GridElementSingleClickHandler GridElementSingleClick;

        //TODO: See https://github.com/cgarciae/karma/issues/1 (the last comment) for layout info
        // Essentially, a layout class needs to be created that stores the rootUI, etc.
	    
	    [Inject] [UsedImplicitly]
	    public void Constructor(ILogger logger, TilePresenter.Factory tileFactory, ILayeredGrid<MapItem> layers, MapItemFactory factory) {
		    Logger = logger;
		    TileFactory = tileFactory;
		    _miFactory = factory;
		    LayeredGrid = layers;
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
						feature.GameObject = DecorateNew(TileFactory.Create(), feature.Value, "Geometry", geometryContainer).gameObject;
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
						piece.GameObject = DecorateNew(TileFactory.Create(), piece.Value, "Entities", entityContainer).gameObject;
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
					MapItem blank = _miFactory.CreateBlankTile();
					
					blank.SetPosition(row, column, true, false);

					piece.GameObject = DecorateNew(TileFactory.Create(), blank, "Overlay", overlayContainer,
						(tile, obj) => {
							GridElementSingleClick?.Invoke(tile, obj);
						}).gameObject;
				}
			}
		}

	    private void HandleChangedGridEvent(object obj) {
		    if(obj is GridCollectionEventArgs<MapItem> conv)
				HandleChangedGridEvent(conv);
		}

	    private void HandleChangedGridEvent(GridCollectionEventArgs<MapItem> args) {
		    if (args.GridPiece.GameObject == null) return;
		    //Logger.Log($"Event triggered for {args.GridPiece.Position} - its new value is {args.GridPiece.Value}");
		    DecorateExisting(args.GridPiece);
	    }

	    private void DecorateExisting(GridPiece<MapItem> piece) {
		    TilePresenter tile = piece.GameObject.GetComponent<TilePresenter>();
		    if(tile == null) {
			    Logger.Log($"This GridPiece has no TilePresenter attached! @{piece.GameObject.name}", LogLevels.WARNING);
			    return;
		    }

		    tile.MapItem = piece.Value;
	    }

	    private TilePresenter DecorateNew(TilePresenter tile, MapItem mi, string sortingLayer, GameObject parent, TilePresenter.SingleClickEventHandler clickHandler = null) {
			tile.MapItem = mi;

		    tile.transform.position = Vector3.zero;
			tile.transform.rotation = Quaternion.identity;
		    
			if (parent != null)
				tile.transform.SetParent(parent.transform, false);
			if (!string.IsNullOrEmpty(sortingLayer))
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
