using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

[ExecuteInEditMode]
public class Main : MonoBehaviour {

    public float GRID_COLUMN_SPACER;
    public float GRID_ROW_SPACER;

	public bool dumpLevelXML;
	public bool dumpEntityXML;
	public bool dumpProgramXML;
	public bool dumpEnemyXML;
    public bool dumpProgramAbilities;
	public bool dumpGeometry;
	public bool dumpEntities;
	public bool dumpTileSets;
	public bool dumpEntityRasters;

    public IEnumerable<SoftwareTool> Tools; 
    public IEnumerable<SoftwareTool> SentryTools; 
    public List<SoftwareTool> AllSoftwareTools;

    private Map map;
    private Dictionary<int, SoftwareTool> entityIDs = new Dictionary<int, SoftwareTool>();
    private List<List<int>> geometryRow = new List<List<int>>();
    private List<List<int>> entityRow = new List<List<int>>();

    // Use this for initialization

    [UsedImplicitly]
    void Start () {
        InitialiseData();

        Dictionary<string, Sprite> entitySpriteDictionary = new Dictionary<string, Sprite>() {
            { "blank",  Resources.Load<Sprite>("Sprites/map_features/empty") }
        };

        foreach(SoftwareTool software in AllSoftwareTools) {
            if(!entitySpriteDictionary.ContainsKey(software.string_id)) {
                Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", software.string_id));
                entitySpriteDictionary.Add(software.string_id, sprite);
            }
        }

        // Start geometry

        Sprite path = Resources.Load<Sprite>("Sprites/map_features/path");
        Sprite wall = Resources.Load<Sprite>("Sprites/map_features/wall");

        float rowSpacer = 0;

        GameObject geometryContainer = new GameObject("Geometry Grid");

        for (int row = 0; row < geometryRow.Count; row++) {
            float columnSpacer = 0;

            GameObject rowContainer = new GameObject();
            rowContainer.transform.parent = geometryContainer.transform;
            rowContainer.name = "Geometry Row " + (row + 1) + " [Generated]";
            rowContainer.hideFlags = HideFlags.DontSave;

            for (int column = 0; column < geometryRow[1].Count; column++)
            {
                //Utility.UnityLog("(" + (row+1).ToString() + "," + (column+1) + ") id is "+ GetEntityByCoords(row, column));
                int featureID = GetGeometryByCoords(row, column);
                if (featureID != 0) {
                    SpriteRenderer rendered_tile = new GameObject().AddComponent<SpriteRenderer>();
                    rendered_tile.transform.parent = rowContainer.transform;
                    rendered_tile.hideFlags = HideFlags.DontSave;

                    Sprite sprite;
                    bool foundSprite = entitySpriteDictionary.TryGetValue(entityIDs[featureID].string_id, out sprite);

                    if(!foundSprite)
                        Utility.UnityLog("Failed for " + entityIDs[featureID].string_id);

                    rendered_tile.sprite = sprite;
                    rendered_tile.name = "Grid Path";
                    rendered_tile.transform.localScale += new Vector3(2f, 2f, 2f);
                    rendered_tile.transform.Translate(new Vector3(columnSpacer, rowSpacer));
                }
                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }

        // End geometry

        // Start entity

        Utility.UnityLog(entitySpriteDictionary.Count);

        rowSpacer = 0;

        GameObject entityContainer = new GameObject("Geometry Grid");

        for(int row = 0; row < entityRow.Count; row++) {
            float columnSpacer = 0;

            GameObject rowContainer = new GameObject();
            rowContainer.name = "Geometry Row " + (row + 1) + " [Generated]";
            rowContainer.transform.parent = entityContainer.transform;
            rowContainer.hideFlags = HideFlags.DontSave;

            for(int column = 0; column < entityRow[1].Count; column++) {
                KeyValuePair<int, SoftwareTool> tool = GetEntityByCoords(row, column);

                if (tool.Value != null) {
                    Sprite success;
                    if (!entitySpriteDictionary.TryGetValue(tool.Value.string_id, out success)) {
                        Utility.UnityLog("Something is wrong with <b>" + tool.Value.string_id + "</b>");
                        continue;
                    }
                    if (success == null) {
                        Utility.UnityLog("Something is wrong with <b>" + tool.Value.string_id + "</b>");
                        continue;
                    }

                    SpriteRenderer renderedTile = new GameObject().AddComponent<SpriteRenderer>();
                    Utility.UnityLog("Nothing is wrong with" + tool.Value.string_id);
                    renderedTile.sprite = success;
                    renderedTile.name = tool.Value.name;
                    renderedTile.transform.localScale += new Vector3(2f, 2f, 2f);
                    renderedTile.transform.Translate(new Vector3(columnSpacer, rowSpacer));
                }
                columnSpacer += GRID_COLUMN_SPACER;
            }
            rowSpacer += GRID_ROW_SPACER;
        }

        // End entity

    }

    KeyValuePair<int, SoftwareTool> GetEntityByCoords(int x, int y)
    {
        int id = entityRow[x][y];
        SoftwareTool success;
        entityIDs.TryGetValue(id, out success);

        return new KeyValuePair<int, SoftwareTool>(id, success);
    }

    int GetGeometryByCoords(int x, int y) {
        return geometryRow[x][y];
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
        if(Utility.level == LogLevels.DEBUG && dumpLevelXML) {
            XmlSerializer serializer = new XmlSerializer(typeof(Map));
            using(TextWriter writer = new StreamWriter(@"./XML_Level_Serialised.xml")) {
                serializer.Serialize(writer, map);
            }
        }
        #endregion

        // Tool/Sentry XML load

        XmlSerializer toolSerializer = new XmlSerializer(typeof(List<SoftwareTool>), new XmlRootAttribute("software"));

        TextReader toolReader = new StreamReader(@"./Assets/Entities/Tools.xml");
        TextReader sentryReader = new StreamReader(@"./Assets/Entities/Sentries.xml");

        Tools = (List<SoftwareTool>)toolSerializer.Deserialize(toolReader);
        SentryTools = (List<SoftwareTool>)toolSerializer.Deserialize(sentryReader);
        AllSoftwareTools = Tools.Concat(SentryTools).ToList();

        toolReader.Close();
        sentryReader.Close();

        #region debug serialised SoftwareXML & Abilities

        foreach(SoftwareTool tool in Tools) {
            if(dumpProgramXML)
                Utility.UnityLog(tool.name);
            if(dumpProgramAbilities)
                foreach(var ability in tool.attacks) {
                    Utility.UnityLog("---->" + ability.name);
                }
        }

        if(dumpEnemyXML)
            foreach(SoftwareTool tool in SentryTools) {
                Utility.UnityLog(tool.name);
            }

        if(Utility.level == LogLevels.DEBUG && dumpProgramXML) {
            using(TextWriter writer = new StreamWriter(@"./XML_Tools_Serialised.xml")) {
                toolSerializer.Serialize(writer, Tools);
            }

            using(TextWriter writer = new StreamWriter(@"./XML_Sentries_Serialised.xml")) {
                toolSerializer.Serialize(writer, SentryTools);
            }
        }



        Utility.UnityLog("Starting to parse Tilesets", LogLevels.INFO);

        #endregion

        foreach(TileSet ts in map.tilesets) {
            Utility.UnityLog("Currently working on the TS " + ts.name, LogLevels.DEBUG);
            // Tiler's tiles are zero-index ID'd, but this is referenced in the actual map data one-indexed.
            // So, an empty tile will always be zero, and therefore we need to add the firstGID to the ID to get our actual ID.
            int tileFirstGID = ts.firstgid;

            Utility.UnityLog("First GID of " + ts.name + " is " + tileFirstGID, LogLevels.DEBUG);
            foreach(Tile tl in ts.tiles) {
                TileSetProperty tileStringIDProperty = tl.properties.First(x => x.name == "id");

                string[] test = AllSoftwareTools.Select(x => x.string_id).ToArray();

                SoftwareTool existing = AllSoftwareTools.FirstOrDefault(x => x.string_id == tileStringIDProperty.value);
                if(existing != null)
                    entityIDs.Add(tl.id + tileFirstGID, existing);
                else
                    Utility.UnityLog("Failed to add ID " + tileStringIDProperty.value);
                if(dumpTileSets)
                    Utility.UnityLog("This tile's " + tileStringIDProperty.name + " is '" + tileStringIDProperty.value + "'", LogLevels.DEBUG);
            }
        }

        Utility.UnityLog("Finished parsing Tilesets");

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
                if(Utility.level == LogLevels.DEBUG && dumpGeometry) {
                    Utility.UnityLog("Printing all Geometry values", LogLevels.DEBUG);
                    for(int row = 0; row < geometryRow.Count; row++) {
                        int pleasantRow = row + 1;
                        //Utility.UnityLog("Geo. Row " + pleasantRow.ToString(), LogLevels.DEBUG);
                        for(int column = 0; column < geometryRow[row].Count; column++) {
                            Utility.UnityLog("Geo. (" + pleasantRow + ", " + (column + 1) + ") " + geometryRow[row][column], LogLevels.DEBUG);
                        }
                    }
                }


                Utility.UnityLog("Finished parsing Geometry layer");
            } else if(l.name == "Entities") {
                Utility.UnityLog("Starting to parse Entities layer");

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
                    entityRow.Add(columnList);
                }

                #region debugEntityValues
                if(Utility.level == LogLevels.DEBUG && dumpEntities) {
                    Utility.UnityLog("Printing all Entitity values", LogLevels.DEBUG);
                    for(int row = 0; row < entityRow.Count; row++) {
                        int pleasantRow = row + 1;
                        Utility.UnityLog("Ent. Row " + pleasantRow, LogLevels.DEBUG);
                        for(int column = 0; column < entityRow[row].Count; column++) {
                            Utility.UnityLog("Ent. " + entityRow[row][column], LogLevels.DEBUG);
                        }
                    }
                }
                #endregion

                Utility.UnityLog("Finished parsing Entity layer");
            }
        }

        #region debugEntityRasters

        Utility.UnityLog("Displaying loaded entities");
        if(dumpEntityRasters) {
            int tile = 1;
            for(int row = 0; row < entityRow.Count; row++) {
                for(int column = 0; column < entityRow[row].Count; column++) {
                    Utility.UnityLog(tile + ": " + entityIDs[entityRow[row][column]]);
                    tile++;
                }
            }
        }

        #endregion
    }
}
