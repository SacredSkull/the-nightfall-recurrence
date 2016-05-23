using UnityEngine;

using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;
using Level;
using Utility;
using Level.Entity;
using UnityEngine.UI;
using Utility.Collections.Grid;
using Logger = Utility.Logger;

public class GameController : MonoBehaviour {
	public bool dumpLevelXML;
	public bool dumpEntityXML;
	public bool dumpProgramXML;
	public bool dumpEnemyXML;
    public bool dumpFeatureXML;
    public bool dumpLoadedMapItems;
    public bool dumpProgramAbilities;
	public bool dumpGeometry;
	public bool dumpEntities;
	public bool dumpTileSets;
	public bool dumpEntityRasters;
    // TODO: This should really be on the GridPiece prefab (and the prefab should probably be called SoftwareTool and have the relevant object attached).
    public AudioClip MovementClip;

    private static int GRID_ROWS = 12;
    private static int GRID_COLUMNS = 16;

    public IEnumerable<HackTool> Tools; 
    public IEnumerable<Sentry> SentryTools; 
    public List<SoftwareTool> AllSoftware;
    public List<MapItem> AllFeatures;
    public List<Sentry> LevelSentries = new List<Sentry>();
    public List<SoftwareTool> LevelSoftwareTools = new List<SoftwareTool>();
    public event DataLoadedHandler DataLoaded;
    public delegate void DataLoadedHandler();

    private Map map;
    private static readonly Dictionary<int, MapItem> mapItems = new Dictionary<int, MapItem>();
    private static readonly GridPiece<MapItem> EmptyMapItem = new GridPiece<MapItem>(0, MapItem.BlankTile, new Vector2(), 1, null);
    private static readonly LayeredGrid<MapItem> LayeredGrid = new LayeredGrid<MapItem>(EmptyMapItem, GRID_ROWS, GRID_COLUMNS);

    private static readonly GridCollection<MapItem> geometryGrid = LayeredGrid.Add("geometry");
    private static readonly GridCollection<MapItem> entityGrid = LayeredGrid.Add("entity");
    private static readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

    public GameObject gridContainer;
    public GameObject gridPiece;


    private SoftwareTool test;
    private GridGraph<MapItem> graph;

    private void Start () {
        // Load from XML
        ReadData();

        // Using XML data, map sprites to appropriate entities
        // MUST take place AFTER loading, but BEFORE grid injection
        LoadSprites();

        // Generate level from geometry/entity data
        InjectGeometry();
        InjectEntities();

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

        ServiceLocator.ProvideGraph(graph);
        ServiceLocator.ProvideLevelLayeredGrid(LayeredGrid);

        entityGrid.PieceChanged += HandleChangedGridEvent;
        geometryGrid.PieceChanged += HandleChangedGridEvent;

        DataLoaded?.Invoke();

        test = (SoftwareTool)entityGrid.Get(1, 4).Value;
    }

    // Update is called once per frame
    private void Update() {
//        if (Input.GetMouseButtonDown(0)) {
//            test?.Move(new Vector2(5, 7));
//            Debug.Log(string.Format("Attacking: {0}, Source: {1}", entityGrid.Get(1, 15).Value as Sentry, test));
//            //Debug.Log(test.Attacks);
//            test?.Attacks[0]?.Execute((SoftwareTool)entityGrid.Get(1, 15).Value, test);
//
//            //Debug.Log(string.Format("New position of {0} is {1}", entityGrid.Get(test.GetPosition()).Value.name, entityGrid.Get(test.GetPosition()).Value.GetPosition()));
//        }
    }

    private void HandleChangedGridEvent(GridCollectionEventArgs<MapItem> args) {
        if (args.GridPiece.GameObject != null) {
            //Debug.Log(string.Format("Event triggered for {0} - its new GO is {1}", args.GridPiece.Position, args.NewGO.name));
            DecorateGridPiece(args.GridPiece.GameObject, args.GridPiece);
        }
    }

    private GameObject DecorateGridPiece(GameObject go, GridPiece<MapItem> piece, string sortingLayer = null, GameObject parent = null) {
        SpriteRenderer renderedTile = go.GetComponent<SpriteRenderer>();

        // Optional parameters.
        if (parent != null)
            renderedTile.transform.SetParent(parent.transform, false);
        if (sortingLayer != null && !sortingLayer.Equals(string.Empty))
            renderedTile.sortingLayerName = sortingLayer;

        renderedTile.hideFlags = HideFlags.DontSave;
        renderedTile.sprite = piece.Value.sprite;
        renderedTile.name = piece.Value.name;
        renderedTile.GetComponent<AudioSource>().clip = MovementClip;
        return go;
    }

    private void ReadData() {

        // Test level XML load
        XmlSerializer deserializerLevel = new XmlSerializer(typeof(Map));
        TextReader readerLevel = new StreamReader("./Assets/Levels/1.tmx");
        object objLevel = deserializerLevel.Deserialize(readerLevel);
        map = (Map)objLevel;
        readerLevel.Close();

        #region debug Serialised level XML
        // This is incredibly useful for debugging XML parsing errors.
        // It will create an XML file based on what it has read from the original,
        // meaning that you can quickly spot discrepancies and fix them.
        if(dumpLevelXML) {
            XmlSerializer serializer = new XmlSerializer(typeof(Map));
            using(TextWriter writer = new StreamWriter(@"./XML_Level_Serialised.xml")) {
                serializer.Serialize(writer, map);
            }
        }
        #endregion

        // Tool/Sentry XML load

        XmlSerializer toolSerializer = new XmlSerializer(typeof(List<HackTool>), new XmlRootAttribute("software"));
        XmlSerializer sentrySerializer = new XmlSerializer(typeof(List<Sentry>), new XmlRootAttribute("software"));
        XmlSerializer featureSerializer = new XmlSerializer(typeof(FeaturesXMLRoot), new XmlRootAttribute("features"));

        TextReader toolReader = new StreamReader(@"./Assets/Entities/Tools.xml");
        TextReader sentryReader = new StreamReader(@"./Assets/Entities/Sentries.xml");
        TextReader featureReader = new StreamReader(@"./Assets/Entities/Features.xml");

        Tools = (List<HackTool>)toolSerializer.Deserialize(toolReader);
        SentryTools = (List<Sentry>)sentrySerializer.Deserialize(sentryReader);
        AllSoftware = SentryTools.Cast<SoftwareTool>().Concat(Tools.Cast<SoftwareTool>()).ToList();
        AllFeatures = ((FeaturesXMLRoot)featureSerializer.Deserialize(featureReader)).features;

        mapItems.Add(0, MapItem.BlankTile);
        AllFeatures.Add(MapItem.BlankTile);
        AllFeatures.Add(MapItem.MapPath);
        AllFeatures.Add(SpawnPoint.Spawn);

        toolReader.Close();
        sentryReader.Close();
        featureReader.Close();

        #region debug serialised SoftwareXML & Abilities

        foreach(SoftwareTool tool in Tools) {
            if(dumpProgramXML)
                Logger.UnityLog(tool.name);
            if(dumpProgramAbilities)
                foreach(var ability in tool.Attacks) {
                    Logger.UnityLog("---->" + ability.Name);
                }
        }
        if(dumpFeatureXML)
            foreach (MapItem mapItem in AllFeatures)
            {
                Logger.UnityLog("[FEATURES] Found map feature " + mapItem.name);
            }

        if(dumpFeatureXML)
            using(TextWriter writer = new StreamWriter(@"./XML_Features_Serialised.xml")) {
                featureSerializer.Serialize(writer, new FeaturesXMLRoot() { features = AllFeatures});
            }

        if(dumpEnemyXML)
            foreach(SoftwareTool tool in SentryTools) {
                Logger.UnityLog(tool.name);
            }

        if(dumpProgramXML) {
            using(TextWriter writer = new StreamWriter(@"./XML_Tools_Serialised.xml")) {
                toolSerializer.Serialize(writer, Tools);
            }

            using(TextWriter writer1 = new StreamWriter(@"./XML_Sentries_Serialised.xml")) {
                toolSerializer.Serialize(writer1, SentryTools);
            }
        }

        #endregion

        Logger.UnityLog("Starting to parse Tilesets", Logger.Level.INFO);

        foreach(TileSet ts in map.tilesets) {
            Logger.UnityLog("Currently working on the TS " + ts.name);
            // Tiler's tiles are zero-index ID'd, but this is referenced in the actual map data one-indexed.
            // So, an empty tile will always be zero, and therefore we need to add the firstGID to the ID to get our actual ID.
            int tileFirstGID = ts.firstgid;

            Logger.UnityLog("First GID of " + ts.name + " is " + tileFirstGID);
            foreach(Tile tl in ts.tiles) {
                TileSetProperty tilePropertyStringID = tl.properties.First(x => x.name == "id");

                MapItem existing = AllSoftware.FirstOrDefault(x => x.string_id == tilePropertyStringID.value) ??
                                   AllFeatures.FirstOrDefault(x => x.string_id == tilePropertyStringID.value);

                if(existing != null)
                    mapItems.Add(tl.id + tileFirstGID, existing);
                else
                    Logger.UnityLog(string.Format("Could not find {0} in any of the global lists.", tilePropertyStringID.value), Logger.Level.ERROR);
                if(dumpTileSets)
                    Logger.UnityLog(string.Format("This tile's {0} ({1}) is '{2}'", tilePropertyStringID.name, tl.id + tileFirstGID, tilePropertyStringID.value));
            }
        }

        Logger.UnityLog("Finished parsing Tilesets", Logger.Level.INFO);

        foreach(Layer l in map.layers) {
            if(l.name == "Geometry") {
                Logger.UnityLog("Starting to parse Geometry layer");

                // Begin loop for populating grid geometry
                int currentTile = 0;
                for(int row = 0; row < l.height; row++) {
                    List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
                    for(int column = 0; column < l.width; column++) {
                        LayerTile tile = l.tiles[currentTile++];
                        MapItem mi = new MapItem(mapItems[tile.gid]);
                        mi.SetPosition(row, column);
                        columnList.Add(new GridPiece<MapItem>{ID = tile.gid, Value = mi});
                    }
                    geometryGrid.SetRow(row, columnList);
                }

                // Print the populated grid geometry
                if (dumpGeometry) {
                    Logger.UnityLog("Printing all Geometry values");
                    for(int row = 0; row < geometryGrid.Height; row++) {
                        int pleasantRow = row + 1;
                        //Logger.UnityLog("Geo. Row " + pleasantRow.ToString(), Logger.Level.DEBUG);
                        for(int column = 0; column < geometryGrid.GetRow(row).Count(); column++) {
                            Logger.UnityLog("Geo. (" + pleasantRow + ", " + (column + 1) + ") " + geometryGrid.Get(row, column).ID);
                        }
                    }
                }
                Logger.UnityLog("Finished parsing Geometry layer", Logger.Level.INFO);

            } else if(l.name == "Entities") {
                Logger.UnityLog("Starting to parse Entities layer");

                // Begin loop for populating entity grid
                int currentTile = 0;
                for(int row = 0; row < l.height; row++) {
                    List<GridPiece<MapItem>> columnList = new List<GridPiece<MapItem>>();
                    for (int column = 0; column < l.width; column++) {
                        LayerTile tile = l.tiles[currentTile++];
                        MapItem mi = mapItems[tile.gid];

                        if (mi == null && tile.gid != 0)
                            throw new InvalidCastException(string.Format("A non-entity was found in the entity grid - '{0}'", mapItems[tile.gid].name));

                        MapItem clone = (MapItem) Activator.CreateInstance(mi.GetType(), mi);
                        if(clone is SoftwareTool)
                            LevelSoftwareTools.Add((SoftwareTool)clone);
                        if(clone is Sentry)
                            LevelSentries.Add((Sentry)clone);

                        columnList.Add(new GridPiece<MapItem> { ID = tile.gid, Value =  clone});
                    }
                    entityGrid.SetRow(row, columnList);
                }

                #region debugEntityValues
                if(dumpEntities) {
                    Logger.UnityLog("Printing all Entitity values");
                    for(int row = 0; row < entityGrid.Height; row++) {
                        int pleasantRow = row + 1;
                        Logger.UnityLog("Ent. Row " + pleasantRow);
                        for(int column = 0; column < entityGrid.GetRow(row).Count(); column++) {
                            Logger.UnityLog("Ent. " + entityGrid.Get(row, column).ID);
                        }
                    }
                }
                #endregion

                Logger.UnityLog("Finished parsing Entity layer", Logger.Level.INFO);
            }
        }

        #region debugEntityRasters

        Logger.UnityLog("Displaying loaded entities");
        if(dumpEntityRasters) {
            int tile = 1;
            for(int row = 0; row < entityGrid.Height; row++) {
                for(int column = 0; column < entityGrid.GetRow(row).Count(); column++) {
                    Logger.UnityLog(string.Format("{0} ({1})", tile, mapItems[entityGrid.Get(row, column).ID]));
                    tile++;
                }
            }
        }

        #endregion
    }

    private void InjectGeometry() {

        if(mapItems.Count == 0)
            Logger.UnityLog("mapItems does not contain anything!", Logger.Level.ERROR);

        if(dumpLoadedMapItems)
            foreach(KeyValuePair<int, MapItem> keyvalue in mapItems) {
                Logger.UnityLog(keyvalue.Value.string_id + " has value of " + keyvalue.Key);
            }

        GameObject geometryContainer = (GameObject)Instantiate(gridContainer, Vector3.zero, Quaternion.identity);
        geometryContainer.name = "Geometry Grid";
        geometryContainer.GetComponent<GridLayoutGroup>().constraintCount = GRID_ROWS;

        for (int row = 0; row < geometryGrid.Height; row++) {
            for (int column = 0; column < geometryGrid.GetRow(row).Count(); column++) {
                GridPiece<MapItem> feature = geometryGrid.Get(row, column);
                if (feature.Value == null) {
                    Logger.UnityLog(string.Format("[GEOMETRY][RASTERING] A Grid Piece has a null value! ID: {0}.", feature.ID), Logger.Level.ERROR);
                } else {
                    GameObject newTile = DecorateGridPiece((GameObject)Instantiate(gridPiece, Vector3.zero, Quaternion.identity), feature, "Geometry", geometryContainer);
                    feature.GameObject = newTile;
                }
            }
        }
    }

    private void InjectEntities() {
        Logger.UnityLog("[SPRITE] Holding " + loadedSprites.Count + " sprites.");

        GameObject entityContainer = (GameObject)Instantiate(gridContainer, Vector3.zero, Quaternion.identity);
        entityContainer.name = "Entity Grid";
        entityContainer.GetComponent<GridLayoutGroup>().constraintCount = GRID_ROWS;

        for (int row = 0; row < entityGrid.Height; row++) {
            for (int column = 0; column < entityGrid.GetRow(row).Count(); column++) {
                GridPiece<MapItem> piece = entityGrid.Get(row, column);

                if (piece.Value != null) {
                    GameObject go = DecorateGridPiece((GameObject)Instantiate(gridPiece, Vector3.zero, Quaternion.identity), piece, "Entities", entityContainer);
                    piece.GameObject = go;
                }
            }
        }
    }

    private void LoadSprites() {
        // BEGIN SPRITE LOADING

        foreach (MapItem mi in geometryGrid) {
            if (loadedSprites.ContainsKey(mi.string_id)) {
                mi.sprite = loadedSprites[mi.string_id];
                continue;
            }

            Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/map_features/{0}", mi.string_id));

            if (sprite == null) {
                Logger.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", mi.string_id), Logger.Level.ERROR);
                continue;
            }

            mi.sprite = sprite;
            loadedSprites.Add(mi.string_id, sprite);
        }

        foreach (MapItem mi in entityGrid) {
            if (loadedSprites.ContainsKey(mi.string_id)) {
                mi.sprite = loadedSprites[mi.string_id];
                continue;
            }

            Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", mi.string_id));

            if (sprite == null) {
                Logger.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", mi.string_id), Logger.Level.ERROR);
                continue;
            }

            mi.sprite = sprite;
            loadedSprites.Add(mi.string_id, sprite);
        }

        // END SPRITE LOADING
    }
}
