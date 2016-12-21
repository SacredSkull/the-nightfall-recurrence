using System.Linq;
using Controllers;
using Level;
using thelab.mvc;
using UnityEngine;
using UnityEngine.UI;
using Utility.Collections.Grid;
using Logger = Utility.Logger;

namespace Views {
    public class LevelView : View<ActiveLevel> {
        public GameObject GridPiece;
        public GameObject GridContainer;
	    public ActiveLevel gc;

	    public static GridCollection<MapItem> GeometryLayer;
        public static GridCollection<MapItem> EntityLayer;

        private void Awake() {
	        if (gc) {
		        gc.DataLoaded += (graph, sprites) => {
			        Logger.UnityLog("Beginning to draw grids...", Logger.Level.INFO);
			        GeometryLayer = graph.LayeredGrid.GetLayer("geometry");
			        EntityLayer = graph.LayeredGrid.GetLayer("entity");

			        renderGeometryLayer();
			        renderEntityLayer();

			        GeometryLayer.PieceChanged += HandleChangedGridEvent;
			        EntityLayer.PieceChanged += HandleChangedGridEvent;
		        };
	        } else {
		        Logger.UnityLog("Couldn't register a handler with the Data event!", Logger.Level.ERROR);
	        }
        }

        private void HandleChangedGridEvent(GridCollectionEventArgs<MapItem> args) {
            if (args.GridPiece.GameObject != null) {
                //Debug.Log(string.Format("Event triggered for {0} - its new GO is {1}", args.GridPiece.Position, args.NewGO.name));
                Decorate(args.GridPiece.GameObject, args.GridPiece);
            }
        }

        public void renderGeometryLayer() {
            GameObject geometryContainer = Instantiate(GridContainer, Vector3.zero, Quaternion.identity);
	        geometryContainer.transform.SetParent(this.transform);
            geometryContainer.name = "Geometry Grid";
            geometryContainer.GetComponent<GridLayoutGroup>().constraintCount = GeometryLayer.Height;

            for (int row = 0; row < GeometryLayer.Height; row++) {
                for (int column = 0; column < GeometryLayer.GetRow(row).Count(); column++) {
                    GridPiece<MapItem> feature = GeometryLayer.Get(row, column);
                    if (feature.Value == null) {
                        Logger.UnityLog(string.Format("[GEOMETRY][RASTERING] A Grid Piece has a null value! ID: {0}.",
	                        feature.ID), Logger.Level.ERROR);
                    } else {
                        GameObject newTile = Decorate(Instantiate(GridPiece, Vector3.zero,
	                        Quaternion.identity), feature, "Geometry", geometryContainer);
                        feature.GameObject = newTile;
                    }
                }
            }
        }

        public void renderEntityLayer() {
            GameObject entityContainer = Instantiate(GridContainer, Vector3.zero, Quaternion.identity);
	        entityContainer.transform.SetParent(this.transform);
	        entityContainer.name = "Entity Grid";
            entityContainer.GetComponent<GridLayoutGroup>().constraintCount = EntityLayer.Height;

            for (int row = 0; row < EntityLayer.Height; row++) {
                for (int column = 0; column < EntityLayer.GetRow(row).Count(); column++) {
                    GridPiece<MapItem> piece = EntityLayer.Get(row, column);

                    if (piece.Value != null) {
	                    if(piece.ID != 0 && piece.Value.sprite == null)
		                    Logger.UnityLog(string.Format("[ENTITY][RASTERING] An entity has no sprite attached! ID: {0}.",
			                    piece.ID), Logger.Level.ERROR);
	                    GameObject go = Decorate(Instantiate(GridPiece, Vector3.zero, Quaternion.identity),
	                        piece, "Entities", entityContainer);
                        piece.GameObject = go;
                    }
                }
            }
        }

	    public static GameObject Decorate(GameObject go, GridPiece<MapItem> piece, string sortingLayer = null, GameObject parent = null) {
		    SpriteRenderer renderedTile = go.GetComponent<SpriteRenderer>();

		    // Optional parameters.
		    if (parent != null)
			    renderedTile.transform.SetParent(parent.transform, false);
		    if (sortingLayer != null && !sortingLayer.Equals(string.Empty))
			    renderedTile.sortingLayerName = sortingLayer;

		    //Logger.UnityLog(string.Format("[ENTITY][RASTERING] Sprite attached ID: {0}.",
			//    piece.Value.sprite), Logger.Level.ERROR);


		    renderedTile.hideFlags = HideFlags.DontSave;
		    renderedTile.sprite = piece.Value.sprite;
		    renderedTile.name = piece.Value.name;

		    return go;
	    }
    }
}
