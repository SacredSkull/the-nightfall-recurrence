using System;
using UnityEngine;

using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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

    private Map map;
    private readonly Dictionary<int, MapItem> mapItems = new Dictionary<int, MapItem>();
    private readonly List<List<int>> geometryRow = new List<List<int>>();
    private readonly List<List<int>> entityRow = new List<List<int>>();

    [UsedImplicitly]
    void Start () {
        InitialiseData();

        Dictionary<string, Sprite> entitySpriteDictionary = new Dictionary<string, Sprite>() {
            { "blank",  Resources.Load<Sprite>("Sprites/map_features/empty") },
        };

        // BEGIN SPRITE LOADING

        foreach(MapItem mapItem in AllFeatures) {
            if(!entitySpriteDictionary.ContainsKey(mapItem.string_id)) {
                Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/map_features/{0}", mapItem.string_id));
                if(sprite == null) {
                    Utility.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", mapItem.string_id), Utility.Level.ERROR);
                    continue;
                }
                entitySpriteDictionary.Add(mapItem.string_id, sprite);
            }
        }

        foreach(SoftwareTool software in AllSoftwareTools) {
            if(!entitySpriteDictionary.ContainsKey(software.string_id)) {
                Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", software.string_id));
                if(sprite == null) {
                    Utility.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", software.string_id), Utility.Level.ERROR);
                    continue;
                }
                entitySpriteDictionary.Add(software.string_id, sprite);
            }
        }

        // END SPRITE LOADING

        // Start geometry
        float rowSpacer = 0;

        if(mapItems.Count == 0)
            Utility.UnityLog("mapItems does not contain anything!", Utility.Level.ERROR);

        if(dumpLoadedMapItems)
            foreach(KeyValuePair<int, MapItem> keyvalue in mapItems) {
                Utility.UnityLog(keyvalue.Value.string_id + " has value of " + keyvalue.Key);
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
                //Utility.UnityLog("(" + (row+1).ToString() + "," + (column+1) + ") id is "+ GetEntityByCoords(row, column));
                KeyValuePair<int, MapItem> feature = GetGeometryByCoords(row, column);

                if (feature.Key != 0) {
                    if (feature.Value == null)
                    {
                        Utility.UnityLog(string.Format("[GEOMETRY][RASTERING] {0} could not be located in the geometry grid.", feature.Key), Utility.Level.ERROR);
                    }
                    else
                    {
                        SpriteRenderer renderedTile = new GameObject().AddComponent<SpriteRenderer>();
                        renderedTile.transform.parent = rowContainer.transform;
                        renderedTile.hideFlags = HideFlags.DontSave;

                        Sprite sprite;
                        entitySpriteDictionary.TryGetValue(feature.Value.string_id, out sprite);

                        renderedTile.sprite = sprite;
                        renderedTile.name = "Grid Path";
                        renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                        renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer));
                    }
                }
                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }

        // End geometry

        // Start entity

        Utility.UnityLog("[SPRITE] Holding " + entitySpriteDictionary.Count + " sprites.");

        rowSpacer = 0;

        GameObject entityContainer = new GameObject("Entity Grid");

        for(int row = 0; row < entityRow.Count; row++) {
            float columnSpacer = 0;

            GameObject rowContainer = new GameObject {name = "Entity Row " + (row + 1) + " [Generated]"};
            rowContainer.transform.parent = entityContainer.transform;
            rowContainer.hideFlags = HideFlags.DontSave;

            for(int column = 0; column < entityRow[0].Count; column++) {
                KeyValuePair<int, MapItem> mapEntity = GetEntityByCoords(row, column);

                if (mapEntity.Value != null) {
                    Sprite success;
                    if (!entitySpriteDictionary.TryGetValue(mapEntity.Value.string_id, out success)) {
                        Utility.UnityLog("Something is wrong with <b>" + mapEntity.Value.string_id + "</b>", Utility.Level.ERROR);
                        success = entitySpriteDictionary["empty"];
                    }
                    if (success == null) {
                        Utility.UnityLog("Something is wrong with <b>" + mapEntity.Value.string_id + string.Format("</b> at row: {0}, column: {1}", row, column), Utility.Level.ERROR);
                        success = entitySpriteDictionary["empty"];
                    }

                    SpriteRenderer renderedTile = new GameObject().AddComponent<SpriteRenderer>();
                    renderedTile.sprite = success;
                    renderedTile.name = mapEntity.Value.name;
                    renderedTile.transform.parent = rowContainer.transform;
                    renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                    renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer, -1f));
                }
                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }

        // End entity

    }

    KeyValuePair<int, MapItem> GetEntityByCoords(int x, int y)
    {
        int id = entityRow[x][y];
        MapItem success;
        mapItems.TryGetValue(id, out success);

        if (id != 0) {
            // Ensure that this is infact an entity, and not a general map object
            if (success is SoftwareTool)
                return new KeyValuePair<int, MapItem>(id, success);
            Utility.UnityLog(
                string.Format(
                    "[ENTITY] Attempted to load a map feature ({0}) into the entity layer! Check that it is on the right layer in Tiled.",
                    success == null ? "NULL mapItem reference - ID #" + id : success.string_id), Utility.Level.WARNING);
        }
        return new KeyValuePair<int, MapItem>(0, null);
    }

    KeyValuePair<int, MapItem> GetGeometryByCoords(int x, int y) {
        int id = geometryRow[x][y];
        MapItem success;
        mapItems.TryGetValue(id, out success);

        if (id != 0) {
            // Ensure this is NOT a software tool
            if (success is SoftwareTool){
                Utility.UnityLog(
                    string.Format(
                        "[ENTITY] Attempted to load software ({0}) into the geometry layer! Check that it is on the right layer in Tiled.",
                        success.string_id), Utility.Level.WARNING);
                return new KeyValuePair<int, MapItem>(0, null);
            }
        }

        return new KeyValuePair<int, MapItem>(id, success);
    }
	
	// Update is called once per frame
	void Update () {
	}

    void InitialiseData() {

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
                Utility.UnityLog(tool.name);
            if(dumpProgramAbilities)
                foreach(var ability in tool.attacks) {
                    Utility.UnityLog("---->" + ability.name);
                }
        }
        if(dumpFeatureXML)
            foreach (MapItem mapItem in AllFeatures)
            {
                Utility.UnityLog("[FEATURES] Found map feature " + mapItem.name);
            }

        if(dumpFeatureXML)
            using(TextWriter writer = new StreamWriter(@"./XML_Features_Serialised.xml")) {
                featureSerializer.Serialize(writer, new FeaturesXMLRoot() { features = AllFeatures});
            }

        if(dumpEnemyXML)
            foreach(SoftwareTool tool in SentryTools) {
                Utility.UnityLog(tool.name);
            }

        if(dumpProgramXML) {
            using(TextWriter writer = new StreamWriter(@"./XML_Tools_Serialised.xml")) {
                toolSerializer.Serialize(writer, Tools);
            }

            using(TextWriter writer = new StreamWriter(@"./XML_Sentries_Serialised.xml")) {
                toolSerializer.Serialize(writer, SentryTools);
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

        Utility.UnityLog("Starting to parse Tilesets", Utility.Level.INFO);



        foreach(TileSet ts in map.tilesets) {
            Utility.UnityLog("Currently working on the TS " + ts.name);
            // Tiler's tiles are zero-index ID'd, but this is referenced in the actual map data one-indexed.
            // So, an empty tile will always be zero, and therefore we need to add the firstGID to the ID to get our actual ID.
            int tileFirstGID = ts.firstgid;

            Utility.UnityLog("First GID of " + ts.name + " is " + tileFirstGID);
            foreach(Tile tl in ts.tiles) {
                TileSetProperty tilePropertyStringID = tl.properties.First(x => x.name == "id");

                //string[] test = AllSoftwareTools.Select(x => x.string_id).ToArray();

                MapItem existing = AllSoftwareTools.FirstOrDefault(x => x.string_id == tilePropertyStringID.value) ??
                                   AllFeatures.FirstOrDefault(x => x.string_id == tilePropertyStringID.value);

                if(existing != null)
                    mapItems.Add(tl.id + tileFirstGID, existing);
                else
                    Utility.UnityLog(string.Format("Could not find {0} in any of the global lists.", tilePropertyStringID.value), Utility.Level.ERROR);
                if(dumpTileSets)
                    Utility.UnityLog("This tile's " + tilePropertyStringID.name + " is '" + tilePropertyStringID.value + "'");
            }
        }

        Utility.UnityLog("Finished parsing Tilesets", Utility.Level.INFO);

        foreach(Layer l in map.layers) {
            if(l.name == "Geometry") {
                Utility.UnityLog("Starting to parse Geometry layer");

                // Begin loop for populating grid geometry
                int currentTile = 0;
                for(int row = 0; row < l.height; row++) {
                    List<int> columnList = new List<int>();
                    for(int column = 0; column < l.width; column++) {
                        LayerTile tile = l.tiles[currentTile];

                        int gid = tile.gid;

                        columnList.Add(gid);
                        currentTile++;
                    }
                    geometryRow.Add(columnList);
                }

                // Print the populated grid geometry
                if(dumpGeometry) {
                    Utility.UnityLog("Printing all Geometry values");
                    for(int row = 0; row < geometryRow.Count; row++) {
                        int pleasantRow = row + 1;
                        //Utility.UnityLog("Geo. Row " + pleasantRow.ToString(), Utility.Level.DEBUG);
                        for(int column = 0; column < geometryRow[row].Count; column++) {
                            Utility.UnityLog("Geo. (" + pleasantRow + ", " + (column + 1) + ") " + geometryRow[row][column]);
                        }
                    }
                }


                Utility.UnityLog("Finished parsing Geometry layer", Utility.Level.INFO);
            } else if(l.name == "Entities") {
                Utility.UnityLog("Starting to parse Entities layer");

                // Begin loop for populating entity grid
                int currentTile = 0;
                for(int row = 0; row < l.height; row++) {
                    List<int> columnList = new List<int>();
                    for(int column = 0; column < l.width; column++) {
                        LayerTile tile = l.tiles[currentTile];

                        int gid = tile.gid;

                        columnList.Add(gid);
                        currentTile++;
                    }
                    entityRow.Add(columnList);
                }

                #region debugEntityValues
                if(dumpEntities) {
                    Utility.UnityLog("Printing all Entitity values");
                    for(int row = 0; row < entityRow.Count; row++) {
                        int pleasantRow = row + 1;
                        Utility.UnityLog("Ent. Row " + pleasantRow);
                        for(int column = 0; column < entityRow[row].Count; column++) {
                            Utility.UnityLog("Ent. " + entityRow[row][column]);
                        }
                    }
                }
                #endregion

                Utility.UnityLog("Finished parsing Entity layer", Utility.Level.INFO);
            }
        }

        #region debugEntityRasters

        Utility.UnityLog("Displaying loaded entities");
        if(dumpEntityRasters) {
            int tile = 1;
            for(int row = 0; row < entityRow.Count; row++) {
                for(int column = 0; column < entityRow[row].Count; column++) {
                    Utility.UnityLog(tile + ": " + mapItems[entityRow[row][column]]);
                    tile++;
                }
            }
        }

        #endregion
    }
}
