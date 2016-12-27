using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Level;
using Level.Entity;
using UnityEngine;
using Utility;
using Utility.Collections.Grid;
using Logger = Utility.Logger;

namespace Models {
	public class LevelModel : DatabaseLoader {
        public static bool dumpLevelXML;
        public static bool dumpGeometry;
        public static bool dumpEntities;
        public static bool dumpTileSets;
        public static bool dumpLevelEntities;
	    public static bool dumpLoadedMapItems;

		public event DataLoadedHandler LevelLoaded;
		public Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
		public List<SoftwareTool> LevelSoftwareTools = new List<SoftwareTool>();
	    public List<Sentry> LevelSentries = new List<Sentry>();
	    public static readonly Dictionary<int, MapItem> mapItems = new Dictionary<int, MapItem>();
	    public GridGraph<MapItem> graph;

	    private LayeredGrid<MapItem> LayeredGrid;
	    private GridCollection<MapItem> geometryGrid;
	    private GridCollection<MapItem> entityGrid;
	    private Map map;
	    private static readonly GridPiece<MapItem> EmptyMapItem = new GridPiece<MapItem>(
	        0, MapItem.BlankTile, new Vector2(), 1, null
        );
	    private string path = @"/Levels/1.tmx";
        private EntityModel em;

        /* TODO: Stopped here 15/12/16 - stripping down ActiveLevel - moving the code for loading the map level itself into the this class, and the entities into the MapItemsModel - EntityModel sounds like a better name for that btw. Additionally, need to add wrapper bools for dumpWhatever so it appears in Unity's editor.  */

		public LevelModel(IDatabase db) : base(db) { }

		[Obsolete("Use Load(path) instead!")]
        public override void Load() {
	        // If given nothing, then use the default (and log a warning - it's not really meant to be used!)
	        Logger.UnityLog("Wasn't given a level to load!", Logger.Level.WARNING);
	        Load(path);
        }

		public void Load(string path) {
		    this.path = path;
		    em = new EntityModel(db);
		    em.EntitiesLoaded += LoadMap;
		    em.Load();
			//Sprites = sprites.Load(em.AllFeatures.Concat(em.AllSoftware.Cast<MapItem>()).ToList());
		}

		private void LoadMap() {
	        // TODO: actually load a passed level
	        map = db.loadLevel(path);

	        LayeredGrid = new LayeredGrid<MapItem>(EmptyMapItem, map.height, map.width);
	        geometryGrid = LayeredGrid.Add("geometry");
	        entityGrid = LayeredGrid.Add("entity");

            mapItems[0] = MapItem.BlankTile;

            Logger.UnityLog("Starting to parse Tilesets", Logger.Level.INFO);

            foreach (TileSet ts in map.tilesets) {
                Logger.UnityLog("Currently working on the TS " + ts.name);
                // Tiler's tiles are zero-index ID'd, but this is referenced in the actual map data one-indexed.
                // So, an empty tile will always be zero, and therefore we need to add the firstGID to the ID
	            // to get our actual ID.
                int tileFirstGID = ts.firstgid;

                Logger.UnityLog("First GID of " + ts.name + " is " + tileFirstGID);
                foreach (Tile tl in ts.tiles) {
                    TileSetProperty tilePropertyStringID = tl.properties.First(x => x.name == "id");

                    MapItem existing = em.AllSoftware.FirstOrDefault(x => x.string_id == tilePropertyStringID.value) ??
                                       em.AllFeatures.FirstOrDefault(x => x.string_id == tilePropertyStringID.value);

                    if (existing != null)
                        mapItems[tl.id + tileFirstGID] = existing;
                    else
                        Logger.UnityLog(string.Format("Could not find {0} in any of the global lists.",
		                        tilePropertyStringID.value), Logger.Level.ERROR);
                    if (dumpTileSets)
                        Logger.UnityLog(string.Format("This tile's {0} ({1}) is '{2}'", tilePropertyStringID.name,
		                        tl.id + tileFirstGID, tilePropertyStringID.value));
                }
            }

            Logger.UnityLog("Finished parsing Tilesets", Logger.Level.INFO);

            foreach (Layer l in map.layers) {
                if (l.name == "Geometry") {
                    Logger.UnityLog("Starting to parse Geometry layer");

                    // Begin loop for populating grid geometry
                    int currentTile = 0;
                    for (int row = 0; row < l.height; row++) {
                        List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
                        for (int column = 0; column < l.width; column++) {
                            LayerTile tile = l.tiles[currentTile++];
                            MapItem mi = new MapItem(mapItems[tile.gid]);
                            mi.SetPosition(row, column);
                            columnList.Add(new GridPiece<MapItem> { ID = tile.gid, Value = mi });
                        }
                        geometryGrid.SetRow(row, columnList);
                    }

                    // Print the populated grid geometry
                    if (dumpGeometry) {
                        Logger.UnityLog("Printing all Geometry values");
                        for (int row = 0; row < geometryGrid.Height; row++) {
                            int pleasantRow = row + 1;
                            //Logger.UnityLog("Geo. Row " + pleasantRow.ToString(), Logger.Level.DEBUG);
                            for (int column = 0; column < geometryGrid.GetRow(row).Count(); column++) {
                                Logger.UnityLog("Geo. (" + pleasantRow + ", " + (column + 1) + ") " +
                                                geometryGrid.Get(row, column).ID);
                            }
                        }
                    }
                    Logger.UnityLog("Finished parsing Geometry layer", Logger.Level.INFO);

                }
                else if (l.name == "Entities") {
                    Logger.UnityLog("Starting to parse Entities layer");

                    // Begin loop for populating entity grid
                    int currentTile = 0;
                    for (int row = 0; row < l.height; row++) {
                        List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
                        for (int column = 0; column < l.width; column++) {
                            LayerTile tile = l.tiles[currentTile++];
                            MapItem mi = mapItems[tile.gid];

                            if (mi == null && tile.gid != 0)
                                throw new InvalidCastException(
                                    $"A non-entity was found in the entity grid - '{mapItems[tile.gid].name}'");

                            MapItem clone = (MapItem)Activator.CreateInstance(mi.GetType(), mi);
                            if (clone is SoftwareTool)
                                LevelSoftwareTools.Add((SoftwareTool)clone);
                            if (clone is Sentry)
                                LevelSentries.Add((Sentry)clone);

                            columnList.Add(new GridPiece<MapItem> { ID = tile.gid, Value = clone });
                        }
                        entityGrid.SetRow(row, columnList);
                    }

                    #region debugEntityValues
                    if (dumpEntities) {
                        Logger.UnityLog("Printing all Entitity values");
                        for (int row = 0; row < entityGrid.Height; row++) {
                            int pleasantRow = row + 1;
                            Logger.UnityLog("Ent. Row " + pleasantRow);
                            for (int column = 0; column < entityGrid.GetRow(row).Count(); column++) {
                                Logger.UnityLog("Ent. " + entityGrid.Get(row, column).ID);
                            }
                        }
                    }
                    #endregion

                    Logger.UnityLog("Finished parsing Entity layer", Logger.Level.INFO);
                }
            }

	        if (mapItems.Count == 0)
		        Logger.UnityLog("mapItems does not contain anything!", Logger.Level.ERROR);
	        if (dumpLoadedMapItems)
		        foreach(KeyValuePair<int, MapItem> keyvalue in mapItems) {
			        Logger.UnityLog(keyvalue.Value.string_id + " has value of " + keyvalue.Key);
		        }

	        HashSet<int> goodGeoIDs = new HashSet<int>();

	        foreach (var pair in mapItems) {
		        if (pair.Value.string_id == MapItem.MapPath.string_id)
			        goodGeoIDs.Add(pair.Key);
		        if (pair.Value.string_id == SpawnPoint.Spawn.string_id)
			        goodGeoIDs.Add(pair.Key);
	        }

	        // Might allow certain programs to be phased through
	        HashSet<int> goodEntIDs = new HashSet<int>();

	        foreach (var pair in mapItems) {
		        if (pair.Value.string_id == MapItem.BlankTile.string_id)
			        goodEntIDs.Add(pair.Key);
		        if (pair.Value.string_id == SpawnPoint.Spawn.string_id)
			        goodEntIDs.Add(pair.Key);
	        }

	        graph = new GridGraph<MapItem>(LayeredGrid, goodGeoIDs, goodEntIDs);

	        // This has to chain with the last DataReady event.
	        Ready();

	        if (dumpLevelEntities) {
		        Logger.UnityLog("Displaying loaded entities");
		        int tile = 1;
		        for (int row = 0; row < entityGrid.Height; row++) {
			        for (int column = 0; column < entityGrid.GetRow(row).Count(); column++) {
				        Logger.UnityLog(string.Format("{0} ({1})", tile, mapItems[entityGrid.Get(row, column).ID]));
				        tile++;
			        }
		        }
	        }

	        // This is incredibly useful for debugging XML parsing errors.
	        // It will create an XML file based on what it has read from the original,
	        // meaning that you can quickly spot discrepancies and fix them.
	        if(dumpLevelXML) {
		        XmlSerializer serializer = new XmlSerializer(typeof(Map));
		        using(TextWriter writer = new StreamWriter(@"./XML_Level_Serialised.xml")) {
			        serializer.Serialize(writer, map);
		        }
	        }
			ServiceLocator.ProvideGraph(graph);
			ServiceLocator.ProvideLevelLayeredGrid(LayeredGrid);
        }

		protected override void Ready() {
			//throw new NotImplementedException();
			LevelLoaded?.Invoke();
		}


	}
}
