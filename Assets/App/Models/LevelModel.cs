using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Level;
using Level.Entity;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Management;
using Zenject;

namespace Models {
    public class LayerNames {
        public const string ENTITY_LAYER = "entity";
        public const string GEOMETRY_LAYER = "geometry";
        public const string OVERLAY_LAYER = "overlay";
    }

    public class LevelModel : DatabaseLoader {
	    public static bool dumpLevelXML;
        public static bool dumpGeometry;
        public static bool dumpEntities;
        public static bool dumpTileSets;
        public static bool dumpLevelEntities;
	    public static bool dumpLoadedMapItems;

		public readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
	    public List<Sentry> LevelSentries = new List<Sentry>();
	    public List<HackTool> LevelHackTools = new List<HackTool>();
	    public static readonly Dictionary<int, MapItem> mapItems = new Dictionary<int, MapItem>();
	    public GridGraph<MapItem> graph;

	    public LayeredGrid<MapItem> LayeredGrid { get; protected set; }
        private GridCollection<MapItem> GeometryGrid;
        private GridCollection<MapItem> EntityGrid;
        private GridCollection<MapItem> OverlayGrid;
	    private Map map;
	    private static readonly GridPiece<MapItem> EmptyMapItem = new GridPiece<MapItem>(
	        0, MapItem.BlankTile, new Vector2(), 1, null
        );
	    private string path = @"/Levels/2.tmx";
        private EntityModel em;
        /* TODO: Stopped here 15/12/16 - stripping down ActiveLevel - moving the code for loading the map level itself into the this class, and the entities into the MapItemsModel - EntityModel sounds like a better name for that btw. Additionally, need to add wrapper bools for dumpWhatever so it appears in Unity's editor.  */

	    private MapItemFactory _miFactory;
	    //private SoftwareToolFactory _softFactory;
	    private HackToolFactory _htFactory;
	    private SentryFactory _sentFactory;
	    
	    [Inject]
	    public LevelModel(IDatabase db, EntityModel em, MapItemFactory miFactory, HackToolFactory htF, SentryFactory stF) : base(db) {
		    this.em = em;
		    _miFactory = miFactory;
		    _htFactory = htF;
		    _sentFactory = stF;
	    }

		
        [Obsolete("Use Load(path) instead!")]
        public override void Load() {
	        // If given nothing, then use the default (and log a warning - it's not really meant to be used!)
	        logger.Log("Wasn't given a level to load!", LogLevels.WARNING);
	        Load(path);
        }

		public void Load(string path) {
		    this.path = path;
		    em.EntitiesLoaded.Subscribe(LoadMap);
		    em.Load();
		}

		private void LoadMap() {
	        // TODO: actually load a passed level
	        map = db.loadLevel(path);

			LayeredGrid = new LayeredGrid<MapItem>(EmptyMapItem, map.height, map.width);
	        GeometryGrid = LayeredGrid.Add(LayerNames.GEOMETRY_LAYER);
	        EntityGrid = LayeredGrid.Add(LayerNames.ENTITY_LAYER);
		    OverlayGrid = LayeredGrid.Add(LayerNames.OVERLAY_LAYER);

            mapItems[0] = MapItem.BlankTile;

			logger.Log("Starting to parse Tilesets", LogLevels.INFO);

            foreach (TileSet ts in map.tilesets) {
	            logger.Log("Currently working on the TS " + ts.name);
                // Tiler's tiles are zero-index ID'd, but this is referenced in the actual map data one-indexed.
                // So, an empty tile will always be zero, and therefore we need to add the firstGID to the ID
	            // to get our actual ID.
                int tileFirstGID = ts.firstgid;

	            logger.Log("First GID of " + ts.name + " is " + tileFirstGID);
                foreach (Tile tl in ts.tiles) {
                    TileSetProperty tilePropertyStringID = tl.properties.First(x => x.name == "id");

                    MapItem existing = em.AllSoftware.FirstOrDefault(x => x.string_id == tilePropertyStringID.value) ??
                                       em.AllFeatures.FirstOrDefault(x => x.string_id == tilePropertyStringID.value);

                    if (existing != null)
                        mapItems[tl.id + tileFirstGID] = existing;
                    else
	                    logger.Log(string.Format("Could not find {0} in any of the global lists.",
		                        tilePropertyStringID.value), LogLevels.ERROR);
                    if (dumpTileSets)
	                    logger.Log(string.Format("This tile's {0} ({1}) is '{2}'", tilePropertyStringID.name,
		                        tl.id + tileFirstGID, tilePropertyStringID.value));
                }
            }

			logger.Log("Finished parsing Tilesets", LogLevels.INFO);
			
            foreach (Layer l in map.layers) {
                if (l.name == "Geometry") {
	                logger.Log("Starting to parse Geometry layer");

                    // Begin loop for populating grid geometry
                    int currentTile = 0;
                    for (int row = 0; row < l.height; row++) {
                        List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
                        for (int column = 0; column < l.width; column++) {
                            LayerTile tile = l.tiles[currentTile++];
                            MapItem mi = new MapItem(mapItems[tile.gid]);
                            mi.SetPosition(row, column, false);
                            columnList.Add(new GridPiece<MapItem> { ID = tile.gid, Value = mi });
                        }
                        GeometryGrid.SetRow(row, columnList);
                    }

                    // Print the populated grid geometry
                    if (dumpGeometry) {
	                    logger.Log("Printing all Geometry values");
                        for (int row = 0; row < GeometryGrid.Height; row++) {
                            int pleasantRow = row + 1;
                            //Logger.UnityLog("Geo. Row " + pleasantRow.ToString(), Logger.Level.DEBUG);
                            for (int column = 0; column < GeometryGrid.GetRow(row).Count(); column++) {
	                            logger.Log("Geo. (" + pleasantRow + ", " + (column + 1) + ") " +
                                                GeometryGrid.Get(row, column).ID);
                            }
                        }
                    }
	                logger.Log("Finished parsing Geometry layer", LogLevels.INFO);

                }
                else if (l.name == "Entities") {
	                logger.Log("Starting to parse Entities layer");

                    // Begin loop for populating entity grid
                    int currentTile = 0;
                    for (int row = 0; row < l.height; row++) {
                        List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
                        for (int column = 0; column < l.width; column++) {
                            LayerTile tile = l.tiles[currentTile++];
                            MapItem mi;

                            if(!mapItems.TryGetValue(tile.gid, out mi) && tile.gid != 0) {
	                            logger.Log(
                                    $"Tried to access an invalid mapitem (from the global lists), #{tile.gid}",
                                    LogLevels.ERROR);
                                continue;
                            }
	                        MapItem clone = null;
	                        if (mi is Sentry) {
		                        clone = _sentFactory.Create((Sentry) mi);
		                        LevelSentries.Add((Sentry) clone);
	                        } else if (mi is SoftwareTool) {
		                        clone = _htFactory.Create((HackTool) mi);
		                        LevelHackTools.Add((HackTool) clone);
	                        } else
		                        clone = _miFactory.Create(mi);
	                        clone.SetPosition(row, column);
                            columnList.Add(new GridPiece<MapItem> { ID = tile.gid, Value = clone });
                        }
                        EntityGrid.SetRow(row, columnList);
                    }

                    #region debugEntityValues
                    if (dumpEntities) {
	                    logger.Log("Printing all Entitity values");
                        for (int row = 0; row < EntityGrid.Height; row++) {
                            int pleasantRow = row + 1;
	                        logger.Log("Ent. Row " + pleasantRow);
                            for (int column = 0; column < EntityGrid.GetRow(row).Count(); column++) {
	                            logger.Log("Ent. " + EntityGrid.Get(row, column).ID);
                            }
                        }
                    }
                    #endregion

	                logger.Log("Finished parsing Entity layer", LogLevels.INFO);
                }
            }

			
			for (int row = 0; row < Math.Max(EntityGrid.Height, GeometryGrid.Height); row++) {
				List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
				for (int column = 0; column < Math.Max(EntityGrid.Width, GeometryGrid.Width); column++) {
					MapItem clone = _miFactory.CreateBlankTile();
					clone.SetPosition(row, column, false);
					columnList.Add(new GridPiece<MapItem> { ID = 0, Value = clone });
				}
				OverlayGrid.SetRow(row, columnList);
			}

	        if (mapItems.Count == 0)
		        logger.Log("mapItems does not contain anything!", LogLevels.ERROR);
	        if (dumpLoadedMapItems)
		        foreach(KeyValuePair<int, MapItem> keyvalue in mapItems) {
			        logger.Log(keyvalue.Value.string_id + " has value of " + keyvalue.Key);
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
		        logger.Log("Displaying loaded entities");
		        int tile = 1;
		        for (int row = 0; row < EntityGrid.Height; row++) {
			        for (int column = 0; column < EntityGrid.GetRow(row).Count(); column++) {
				        logger.Log(string.Format("{0} ({1})", tile, mapItems[EntityGrid.Get(row, column).ID]));
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
			
			//ServiceLocator.ProvideLevelLayeredGrid(LayeredGrid);
        }

		protected override void Ready() {
			//throw new NotImplementedException();
			SoftwareTool.DeathEvent += (victim, perp, candlestick) => {
				if (victim is HackTool ht)
					LevelHackTools.Remove(ht);
				else
					LevelSentries.Remove((Sentry) victim);
			};
			LoadedEvent.Fire();
		}
	}
}
