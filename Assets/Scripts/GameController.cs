using UnityEngine;

using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Action.Move;
using Assets.Scripts.Utility;
using Utility.Collections;
using Utility.Collections.Grid;
using Logger = Utility.Logger;

public class GameController : MonoBehaviour {

    public float GRID_COLUMN_SPACER;
    public float GRID_ROW_SPACER;

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

    private const int GRID_ROWS = 12;
    private const int GRID_COLUMNS = 16;

    public IEnumerable<SoftwareTool> Tools; 
    public IEnumerable<SoftwareTool> SentryTools; 
    public List<SoftwareTool> AllSoftwareTools;
    public List<MapItem> AllFeatures;
    public GridCollection<SoftwareTool> LevelTools = new GridCollection<SoftwareTool>(); 

    private Map map;
    private static readonly Dictionary<int, MapItem> mapItems = new Dictionary<int, MapItem>();
    public static readonly LayeredGrid<MapItem> LayeredGrid = new LayeredGrid<MapItem> {
        { "geometry", new GridCollection<MapItem>(GRID_ROWS, GRID_COLUMNS) },
        { "entity", new GridCollection<MapItem>(GRID_ROWS, GRID_COLUMNS) }
    };

    private static readonly GridCollection<MapItem> geometryGrid = LayeredGrid[0];
    private static readonly GridCollection<MapItem> entityGrid = LayeredGrid[1];
    private static readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

    private SoftwareTool test;
    private GridGraph<MapItem> testGG;

    void Start () {
        // Load from XML
        ReadData();

        // Using XML data, map sprites to appropriate entities
        // MUST take place AFTER loading, but BEFORE grid injection
        LoadSprites();

        // Generate level from geometry/entity data
        InjectGeometry();
        InjectEntities();

        testGG = new GridGraph<MapItem>(LayeredGrid, new HashSet<int> { 0, 5 });

        test = (SoftwareTool) entityGrid.Get(1, 4).Value;

        StartCoroutine(test.Move(testGG, new Vector2(10, 15), true));
    }

    // Update is called once per frame
    void Update() {
    }

    public static KeyValuePair<int, MapItem> GetEntityByCoords(int x, int y) {
        GridPiece<MapItem> piece = entityGrid.Get(x, y);

        return new KeyValuePair<int, MapItem>(piece.ID, piece.Value);
    }

    public static KeyValuePair<int, MapItem> GetEntityByCoords(Vector2 xy) {
        return GetEntityByCoords((int) xy.x, (int)xy.y);
    }

    public static KeyValuePair<int, MapItem> GetGeometryByCoords(int x, int y) {
        GridPiece<MapItem> piece = geometryGrid.Get(x, y);
        return new KeyValuePair<int, MapItem>(piece.ID, piece.Value);
    }

    public static KeyValuePair<int, MapItem> GetGeometryByCoords(Vector2 xy) {
        return GetGeometryByCoords((int)xy.x, (int)xy.y);
    }

    void ReadData() {

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

        XmlSerializer toolSerializer = new XmlSerializer(typeof(List<SoftwareTool>), new XmlRootAttribute("software"));
        XmlSerializer featureSerializer = new XmlSerializer(typeof(FeaturesXMLRoot), new XmlRootAttribute("features"));

        TextReader toolReader = new StreamReader(@"./Assets/Entities/Tools.xml");
        TextReader sentryReader = new StreamReader(@"./Assets/Entities/Sentries.xml");
        TextReader featureReader = new StreamReader(@"./Assets/Entities/Features.xml");

        Tools = (List<SoftwareTool>)toolSerializer.Deserialize(toolReader);
        SentryTools = (List<SoftwareTool>)toolSerializer.Deserialize(sentryReader);
        AllSoftwareTools = Tools.Concat(SentryTools).ToList();
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
                foreach(var ability in tool.attacks) {
                    Logger.UnityLog("---->" + ability.name);
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

                MapItem existing = AllSoftwareTools.FirstOrDefault(x => x.string_id == tilePropertyStringID.value) ??
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
                        MapItem mi = Cloner.Clone(mapItems[tile.gid]);
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
                        Attackable entity = mapItems[tile.gid] as Attackable;

                        if (entity == null && tile.gid != 0)
                            throw new InvalidCastException(string.Format("A non-entity was found in the entity grid - '{0}'", mapItems[tile.gid].name));
                        columnList.Add(new GridPiece<MapItem> { ID = tile.gid, Value = entity == null ? Cloner.Clone(mapItems[0]) : Cloner.Clone(entity) });
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
                    Logger.UnityLog(string.Format("{0} ({1}): {2}", tile, mapItems[entityGrid.Get(row, column).ID]));
                    tile++;
                }
            }
        }

        #endregion
    }

    private void InjectGeometry() {
        float rowSpacer = 0;

        if(mapItems.Count == 0)
            Logger.UnityLog("mapItems does not contain anything!", Logger.Level.ERROR);

        if(dumpLoadedMapItems)
            foreach(KeyValuePair<int, MapItem> keyvalue in mapItems) {
                Logger.UnityLog(keyvalue.Value.string_id + " has value of " + keyvalue.Key);
            }

        GameObject geometryContainer = new GameObject("Geometry Grid");

        for (int row = 0; row < geometryGrid.Height; row++) {
            float columnSpacer = 0;
            GameObject rowContainer = new GameObject();
            rowContainer.transform.parent = geometryContainer.transform;
            rowContainer.name = "Geometry Row " + (row + 1) + " [Generated]";
            rowContainer.hideFlags = HideFlags.DontSave;

            for (int column = 0; column < geometryGrid.GetRow(row).Count(); column++) {
                KeyValuePair<int, MapItem> feature = GetGeometryByCoords(row, column);

                if (feature.Value == null) {
                    Logger.UnityLog(string.Format("[GEOMETRY][RASTERING] A Grid Piece has a null value! ID: {0}.", feature.Key), Logger.Level.ERROR);
                } else {
                    SpriteRenderer renderedTile = new GameObject().AddComponent<SpriteRenderer>();
                    renderedTile.transform.parent = rowContainer.transform;
                    renderedTile.hideFlags = HideFlags.DontSave;
                    renderedTile.sprite = feature.Value.sprite;
                    renderedTile.name = feature.Value.name;
                    renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                    renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer));
                    feature.Value.gameobject = renderedTile.gameObject;
                }

                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }
    }

    private void InjectEntities() {
        Logger.UnityLog("[SPRITE] Holding " + loadedSprites.Count + " sprites.");

        float rowSpacer = 0;

        GameObject entityContainer = new GameObject("Entity Grid");

        for (int row = 0; row < entityGrid.Height; row++) {
            float columnSpacer = 0;

            GameObject rowContainer = new GameObject { name = "Entity Row " + (row + 1) + " [Generated]" };
            rowContainer.transform.parent = entityContainer.transform;
            rowContainer.hideFlags = HideFlags.DontSave;

            for (int column = 0; column < entityGrid.GetRow(row).Count(); column++) {
                KeyValuePair<int, MapItem> piece = GetEntityByCoords(row, column);

                if (piece.Value != null) {
                    GameObject go = new GameObject(piece.Value.name);
                    piece.Value.gameobject = go;
                    SpriteRenderer renderedTile = go.AddComponent<SpriteRenderer>();
                    renderedTile.sprite = piece.Value.sprite;
                    renderedTile.name = piece.Value.name;
                    renderedTile.transform.parent = rowContainer.transform;
                    renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                    renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer, -1f));
                }

                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
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
