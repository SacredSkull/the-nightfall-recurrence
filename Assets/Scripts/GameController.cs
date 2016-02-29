using UnityEngine;

using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using Scripts.Action.Move;
using System;
using System.Collections;
using Utility.Collections;
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

    public IEnumerable<SoftwareTool> Tools; 
    public IEnumerable<SoftwareTool> SentryTools; 
    public List<SoftwareTool> AllSoftwareTools;
    public List<MapItem> AllFeatures;
    public GridCollection<SoftwareTool> LevelTools = new GridCollection<SoftwareTool>(); 

    private Map map;
    private static readonly Dictionary<int, MapItem> mapItems = new Dictionary<int, MapItem>();
    private static readonly GridCollection<MapItem> geometryRow = new GridCollection<MapItem>();
    private static readonly GridCollection<MapItem> entityRow = new GridCollection<MapItem>();
    private static readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();  

    void Start () {

        mapItems.Add(0, new MapItem { name = "Empty tile", string_id = "blank" });

        // Load from XML
        ReadData();
        // Using XML data, map sprites to appropriate entities
        LoadSprites();

        // Generate level from geometry/entity data
        InjectGeometry();
        InjectEntities();



        GridGraph gg = new GridGraph(geometryRow, new HashSet<int> { 0 });

        foreach (var softwareTool in LevelTools.GridValues) {
            softwareTool.governor.move(gg, softwareTool.gridPosition);
        }
    }

    // TODO: WARNING! This currently gets REFERENCES for entities from the list of loaded entities, meaning that each entity is being assigned the SAME REFERENCE! Set up a cloning/copy operation for a list that guarauntees its uniquity
    public static KeyValuePair<int, Attackable> GetEntityByCoords(int x, int y) {
        int id = entityRow[x][y];
        MapItem success;
        mapItems.TryGetValue(id, out success);

        if (id != 0) {
            // Ensure that this is in fact an entity, and not a general map object
            Attackable value = success as Attackable;
            if (value != null)
                return new KeyValuePair<int, Attackable>(id, value);
            Logger.UnityLog(
                string.Format(
                    "[ENTITY] Attempted to load a map feature ({0}) into the entity layer! Check that it is on the right layer in Tiled.",
                    success == null ? "NULL mapItem reference - ID #" + id : success.string_id), Logger.Level.WARNING);
        }
        return new KeyValuePair<int, Attackable>(0, null);
    }

    // TODO: WARNING! This currently gets REFERENCES for entities from the list of loaded entities, meaning that each entity
    // is being assigned the SAME REFERENCE!
    // Set up a cloning/copy operation for a list that guarauntees its uniquity
    public static KeyValuePair<int, Attackable> GetEntityByCoords(Vector2 xy) {
        int id = entityRow[(int)xy.x][(int)xy.y];
        MapItem success;
        mapItems.TryGetValue(id, out success);

        if (id != 0) {
            // Ensure that this is in fact an entity, and not a general map object
            Attackable value = success as Attackable;
            if (value != null)
                return new KeyValuePair<int, Attackable>(id, value);
            Logger.UnityLog(
                    string.Format(
                            "[ENTITY] Attempted to load a map feature ({0}) into the entity layer! Check that it is on the right layer in Tiled.",
                            success == null ? "NULL mapItem reference - ID #" + id : success.string_id), Logger.Level.WARNING);
        }
        return new KeyValuePair<int, Attackable>(0, null);
    }

    public static KeyValuePair<int, MapItem> GetGeometryByCoords(int x, int y) {
        int id = geometryRow[x][y];
        MapItem success;
        mapItems.TryGetValue(id, out success);

        if (id != 0) {
            // Ensure this is NOT a software tool
            if (success is SoftwareTool){
                Logger.UnityLog(
                    string.Format(
                        "[ENTITY] Attempted to load software ({0}) into the geometry layer! Check that it is on the right layer in Tiled.",
                        success.string_id), Logger.Level.WARNING);
                return new KeyValuePair<int, MapItem>(0, null);
            }
        }

        return new KeyValuePair<int, MapItem>(id, success);
    }

    public static KeyValuePair<int, MapItem> GetGeometryByCoords(Vector2 xy) {
        int id = geometryRow[(int)xy.x][(int)xy.y];
        MapItem success;
        mapItems.TryGetValue(id, out success);

        if (id != 0) {
            // Ensure this is NOT a software tool
            if (success is SoftwareTool){
                Logger.UnityLog(
                        string.Format(
                                "[ENTITY] Attempted to load software ({0}) into the geometry layer! Check that it is on the right layer in Tiled.",
                                success.string_id), Logger.Level.WARNING);
                return new KeyValuePair<int, MapItem>(0, null);
            }
        }

        return new KeyValuePair<int, MapItem>(id, success);
    }

    public static KeyValuePair<int, MapItem> GetMapItemByCoords(Vector2 xy)
    {
        int id = geometryRow[(int)xy.x][(int)xy.y];
        MapItem success;
        mapItems.TryGetValue(id, out success);
        return new KeyValuePair<int, MapItem>(id, success);
    }
	
	// Update is called once per frame
	void Update () {

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

        AllFeatures.Add(new MapItem {
            name = "Map Piece",
            description = "Traversable network pathing",
            string_id = "path"
        });
        AllFeatures.Add(new SpawnPoint {
            name = "Spawn Point",
            description = "Attack vector entry point for your software tools",
            string_id = "spawnpoint",
        });

        Logger.UnityLog("Starting to parse Tilesets", Logger.Level.INFO);



        foreach(TileSet ts in map.tilesets) {
            Logger.UnityLog("Currently working on the TS " + ts.name);
            // Tiler's tiles are zero-index ID'd, but this is referenced in the actual map data one-indexed.
            // So, an empty tile will always be zero, and therefore we need to add the firstGID to the ID to get our actual ID.
            int tileFirstGID = ts.firstgid;

            Logger.UnityLog("First GID of " + ts.name + " is " + tileFirstGID);
            foreach(Tile tl in ts.tiles) {
                TileSetProperty tilePropertyStringID = tl.properties.First(x => x.name == "id");

                //string[] test = AllSoftwareTools.Select(x => x.string_id).ToArray();

                MapItem existing = AllSoftwareTools.FirstOrDefault(x => x.string_id == tilePropertyStringID.value) ??
                                   AllFeatures.FirstOrDefault(x => x.string_id == tilePropertyStringID.value);

                if(existing != null)
                    mapItems.Add(tl.id + tileFirstGID, existing);
                else
                    Logger.UnityLog(string.Format("Could not find {0} in any of the global lists.", tilePropertyStringID.value), Logger.Level.ERROR);
                if(dumpTileSets)
                    Logger.UnityLog("This tile's " + tilePropertyStringID.name + " is '" + tilePropertyStringID.value + "'");
            }
        }

        Logger.UnityLog("Finished parsing Tilesets", Logger.Level.INFO);

        foreach(Layer l in map.layers) {
            if(l.name == "Geometry") {
                Logger.UnityLog("Starting to parse Geometry layer");

                // Begin loop for populating grid geometry
                int currentTile = 0;
                for(int row = 0; row < l.height; row++) {
                    Dictionary<int, int> columnList = new Dictionary<int, int>();
                    for(int column = 0; column < l.width; column++) {
                        LayerTile tile = l.tiles[currentTile];

                        int gid = tile.gid;

                        columnList.Add(column, gid);
                        currentTile++;
                    }
                    geometryRow.Add(row, columnList);
                }

                for (int row = 0; row < l.height; row++)
                {
                    for (int column = 0; column < l.width; column++)
                    {
                        MapItem mi = GetMapItemByCoords(new Vector2(row, column)).Value;
                        mi.gridPosition = new Vector2(row, column);
                    }
                }

                // Print the populated grid geometry
                if (dumpGeometry) {
                    Logger.UnityLog("Printing all Geometry values");
                    for(int row = 0; row < geometryRow.Count; row++) {
                        int pleasantRow = row + 1;
                        //Logger.UnityLog("Geo. Row " + pleasantRow.ToString(), Logger.Level.DEBUG);
                        for(int column = 0; column < geometryRow[row].Count; column++) {
                            Logger.UnityLog("Geo. (" + pleasantRow + ", " + (column + 1) + ") " + geometryRow[row][column]);
                        }
                    }
                }


                Logger.UnityLog("Finished parsing Geometry layer", Logger.Level.INFO);
            } else if(l.name == "Entities") {
                Logger.UnityLog("Starting to parse Entities layer");

                // Begin loop for populating entity grid
                int currentTile = 0;
                for(int row = 0; row < l.height; row++) {
                    Dictionary<int, int> columnList = new Dictionary<int, int>();
                    for(int column = 0; column < l.width; column++) {
                        LayerTile tile = l.tiles[currentTile];

                        int gid = tile.gid;

                        columnList.Add(column, gid);
                        currentTile++;
                    }
                    entityRow.Add(row, columnList);
                }

                for(int row = 0; row < entityRow.Count; row++) {
                    for(int column = 0; column < entityRow[row].Count; column++) {
                        if (entityRow[row][column] == 0)
                            continue;
                        SoftwareTool st = GetEntityByCoords(new Vector2(row, column)).Value as SoftwareTool;
                        if (st == null) continue;
                        SoftwareTool copy = SoftwareTool.Copy(st);
                        copy.gridPosition = new Vector2(row, column);
                        LevelTools.Set(row, column, copy);
                    }
                }

                #region debugEntityValues
                if(dumpEntities) {
                    Logger.UnityLog("Printing all Entitity values");
                    for(int row = 0; row < entityRow.Count; row++) {
                        int pleasantRow = row + 1;
                        Logger.UnityLog("Ent. Row " + pleasantRow);
                        for(int column = 0; column < entityRow[row].Count; column++) {
                            Logger.UnityLog("Ent. " + entityRow[row][column]);
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
            for(int row = 0; row < entityRow.Count; row++) {
                for(int column = 0; column < entityRow[row].Count; column++) {
                    Logger.UnityLog(tile + ": " + mapItems[entityRow[row][column]]);
                    tile++;
                }
            }
        }

        #endregion
    }

    private void LoadSprites() {
        // Load empty tile
        loadedSprites.Add("blank", Resources.Load<Sprite>("Sprites/map_features/empty"));

        // BEGIN SPRITE LOADING

        foreach(MapItem mapItem in AllFeatures) {
            if(loadedSprites.ContainsKey(mapItem.string_id)) {
                mapItem.sprite = loadedSprites[mapItem.string_id];
                continue;
            }

            Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/map_features/{0}", mapItem.string_id));

            if(sprite == null) {
                Logger.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", mapItem.string_id), Logger.Level.ERROR);
                continue;
            }
            mapItem.sprite = sprite;
            loadedSprites.Add(mapItem.string_id, sprite);
        }

        foreach(SoftwareTool software in AllSoftwareTools) {
            if(loadedSprites.ContainsKey(software.string_id)) {
                software.sprite = loadedSprites[software.string_id];
                continue;
            }

            Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", software.string_id));

            if(sprite == null) {
                Logger.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", software.string_id), Logger.Level.ERROR);
                continue;
            }

            software.sprite = sprite;
            loadedSprites.Add(software.string_id, sprite);
        }

        // END SPRITE LOADING
    }

    private void InjectEntities(){
        Logger.UnityLog("[SPRITE] Holding " + loadedSprites.Count + " sprites.");

        float rowSpacer = 0;

        GameObject entityContainer = new GameObject("Entity Grid");

        for(int row = 0; row < entityRow.Count; row++) {
            float columnSpacer = 0;

            GameObject rowContainer = new GameObject {name = "Entity Row " + (row + 1) + " [Generated]"};
            rowContainer.transform.parent = entityContainer.transform;
            rowContainer.hideFlags = HideFlags.DontSave;

            for(int column = 0; column < entityRow[0].Count; column++) {
                KeyValuePair<int, Attackable> mapEntity = GetEntityByCoords(row, column);

                if (mapEntity.Value != null) {
                    SpriteRenderer renderedTile = new GameObject().AddComponent<SpriteRenderer>();
                    renderedTile.sprite = mapEntity.Value.sprite;
                    renderedTile.name = mapEntity.Value.name;
                    renderedTile.transform.parent = rowContainer.transform;
                    renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                    renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer, -1f));
                }
                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }
    }

    private void InjectGeometry(){
        float rowSpacer = 0;

        if(mapItems.Count == 0)
            Logger.UnityLog("mapItems does not contain anything!", Logger.Level.ERROR);

        if(dumpLoadedMapItems)
            foreach(KeyValuePair<int, MapItem> keyvalue in mapItems) {
                Logger.UnityLog(keyvalue.Value.string_id + " has value of " + keyvalue.Key);
            }

        GameObject geometryContainer = new GameObject("Geometry Grid");

        for (int row = 0; row < geometryRow.Count; row++) {
            float columnSpacer = 0;
            GameObject rowContainer = new GameObject();
            rowContainer.transform.parent = geometryContainer.transform;
            rowContainer.name = "Geometry Row " + (row + 1) + " [Generated]";
            rowContainer.hideFlags = HideFlags.DontSave;

            for (int column = 0; column < geometryRow[0].Count; column++)
            {
                KeyValuePair<int, MapItem> feature = GetGeometryByCoords(row, column);

                if (feature.Key != 0) {
                    if (feature.Value == null)
                    {
                        Logger.UnityLog(string.Format("[GEOMETRY][RASTERING] {0} could not be located in the geometry grid.", feature.Key), Logger.Level.ERROR);
                    }
                    else
                    {
                        SpriteRenderer renderedTile = new GameObject().AddComponent<SpriteRenderer>();
                        renderedTile.transform.parent = rowContainer.transform;
                        renderedTile.hideFlags = HideFlags.DontSave;
                        renderedTile.sprite = feature.Value.sprite;
                        renderedTile.name = "Grid Path";
                        renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                        renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer));
                        feature.Value.gameobject = renderedTile.gameObject;
                    }
                } else
                {
                    GameObject emptySquare = new GameObject();
                    emptySquare.name = "Blank tile";
                    emptySquare.transform.localScale += new Vector3(2f, 2f, 2f);
                    emptySquare.transform.Translate(new Vector3(columnSpacer, rowSpacer));
                    feature.Value.gameobject = emptySquare;
                }
                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }
    }
}
