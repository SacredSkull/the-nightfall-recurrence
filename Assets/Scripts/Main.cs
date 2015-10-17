using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

using SacredSkull.Software;

[ExecuteInEditMode]
public class Main : MonoBehaviour {

    public float GRID_COLUMN_SPACER;
    public float GRID_ROW_SPACER;

    Map map;

	public bool dumpLevelXML;
	public bool dumpEntityXML;
	public bool dumpProgramXML;
	public bool dumpEnemyXML;
    public bool dumpProgramAbilities;
	public bool dumpGeometry;
	public bool dumpEntities;
	public bool dumpTileSets;
	public bool dumpEntityRasters;

    private Dictionary<int, SoftwareTool> entityIDs = new Dictionary<int, SoftwareTool>();
    private List<List<int>> geometryRow = new List<List<int>>();
    private List<List<int>> entityRow = new List<List<int>>();

	// Use this for initialization
	void Start () {
        XmlSerializer deserializerLevel = new XmlSerializer(typeof(Map));
        TextReader readerLevel = new StreamReader("./Assets/Levels/1.tmx");
        object objLevel = deserializerLevel.Deserialize(readerLevel);
        map = (Map)objLevel;
        readerLevel.Close();

        #region debug Serialised level XML
        // This is incredibly useful for debugging XML parsing errors.
        // It will create an XML file based on what it has read from the original,
        // meaning that you can quickly spot discrepancies and fix them.
        if (Utility.level == LogLevels.DEBUG && dumpLevelXML) {
			XmlSerializer serializer = new XmlSerializer (typeof(Map)); 
			using (TextWriter writer = new StreamWriter(@"./XML_Level_Serialised.xml")) {
					serializer.Serialize (writer, map); 
			} 
		}
        #endregion

        //Sprite[] sprites = Resources.LoadAll<Sprite>();

        XmlSerializer toolSerializer = new XmlSerializer(typeof(List<SoftwareTool>), new XmlRootAttribute("software"));

        TextReader toolReader = new StreamReader(@"./Assets/Entities/Tools.xml");
        TextReader sentryReader = new StreamReader(@"./Assets/Entities/Sentries.xml");

	    List<SoftwareTool> tools = (List<SoftwareTool>)toolSerializer.Deserialize(toolReader);
	    List<SoftwareTool> sentries = (List<SoftwareTool>)toolSerializer.Deserialize(sentryReader);

        toolReader.Close();
        sentryReader.Close();

        #region debug serialised SoftwareXML & Abilities

        foreach(SoftwareTool tool in tools){
            if(dumpProgramXML)
                Utility.UnityLog(tool.name);
            if(dumpProgramAbilities)
                foreach (var ability in tool.attacks) {
                    Utility.UnityLog("---->" + ability.name);
                }
        }

        if(dumpEnemyXML)
            foreach(SoftwareTool tool in sentries) {
                Utility.UnityLog(tool.name);
            }

        if (Utility.level == LogLevels.DEBUG && dumpProgramXML) {
            using(TextWriter writer = new StreamWriter(@"./XML_Tools_Serialised.xml")) {
                toolSerializer.Serialize(writer, tools);
            }

            using(TextWriter writer = new StreamWriter(@"./XML_Sentries_Serialised.xml")) {
                toolSerializer.Serialize(writer, sentries);
            }
        }

        

        Utility.UnityLog("Starting to parse Tilesets", LogLevels.INFO);

        #endregion

        //entityIDs.Add (0, "empty");

        foreach (TileSet ts in map.tilesets) {
			Utility.UnityLog("Currently working on the TS " + ts.name, LogLevels.DEBUG);
			// For some reason, Tiler doesn't store the first GID as the value actually written to 
			// file. Rather than rely on human based counting, let's use zero-indexed GIDs.
			int tileFirstGID = ts.firstgid;

			Utility.UnityLog("First GID of "+ ts.name +" is " + tileFirstGID, LogLevels.DEBUG);
            foreach (TileSetTile tl in ts.tiles) {
				foreach(TileSetProperty property in tl.properties.propertyList){
					if(property.name == "id"){
						if(dumpTileSets)
							Utility.UnityLog("This tile's "+ property.name + " is '" + property.value + "'", LogLevels.DEBUG);
					}
				}
            }
        }

		Utility.UnityLog("Finished parsing Tilesets", LogLevels.INFO);

        foreach (Layer l in map.layers) {
            if (l.name == "Geometry") {
                Utility.UnityLog("Starting to parse Geometry layer", LogLevels.INFO);

                // Begin loop for populating grid geometry
                int currentTile = 0;
                for (int row = 0; row < l.height; row++) {
                    List<int> columnList = new List<int>();
                    for (int column = 0; column < l.width; column++) {
                        LayerTile tile = l.data.tiles[currentTile];

                        int gid = tile.gid;

                        columnList.Add(gid);
                        currentTile++;
                    }
                    geometryRow.Add(columnList);
                }

                // Print the populated grid geometry
				if(Utility.level == LogLevels.DEBUG && dumpGeometry){
	                Utility.UnityLog("Printing all Geometry values", LogLevels.DEBUG);
	                for (int row = 0; row < geometryRow.Count; row++) {
	                    int pleasantRow = row + 1;
	                    //Utility.UnityLog("Geo. Row " + pleasantRow.ToString(), LogLevels.DEBUG);
	                    for (int column = 0; column < geometryRow[row].Count; column++) {
	                        Utility.UnityLog("Geo. ("+ pleasantRow + ", " + (column+1) + ") " + geometryRow[row][column], LogLevels.DEBUG);
	                    }
	                }
				}
                

                Utility.UnityLog("Finished parsing Geometry layer");
            } else if (l.name == "Entities") {
                Utility.UnityLog("Starting to parse Entities layer");

				// Begin loop for populating grid geometry
				int currentTile = 0;
				for (int row = 0; row < l.height; row++) {
					List<int> columnList = new List<int>();
					for (int column = 0; column < l.width; column++) {
						LayerTile tile = l.data.tiles[currentTile];
						
						int gid = tile.gid;
						
						columnList.Add(gid);
						currentTile++;
					}
					entityRow.Add(columnList);
				}

                #region debugEntityValues
                if(Utility.level == LogLevels.DEBUG && dumpEntities){
					Utility.UnityLog("Printing all Entitity values", LogLevels.DEBUG);
					for (int row = 0; row < entityRow.Count; row++) {
						int pleasantRow = row + 1;
						Utility.UnityLog("Ent. Row " + pleasantRow, LogLevels.DEBUG);
						for (int column = 0; column < entityRow[row].Count; column++) {
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
		if (dumpEntityRasters) {
			int tile = 1;
			for (int row = 0; row < entityRow.Count; row++) {
				for(int column = 0; column < entityRow[row].Count; column++){
					Utility.UnityLog(tile + ": " + entityIDs[entityRow[row][column]]);
					tile++;
				}
			}
		}

        #endregion

        // Start geometry

        Sprite path = Resources.Load<Sprite>("Sprites/map_features/path");
        Sprite wall = Resources.Load<Sprite>("Sprites/map_features/wall");

        float rowSpacer = 0;
        for (int row = 0; row < geometryRow.Count; row++) {
            float columnSpacer = 0;
            for (int column = 0; column < geometryRow[1].Count; column++)
            {
                //Utility.UnityLog("(" + (row+1).ToString() + "," + (column+1) + ") id is "+ GetEntityByCoords(row, column));

                if (GetGeometryByCoords(row, column) != 0) {
                    SpriteRenderer rendered_tile;
                    rendered_tile = new GameObject().AddComponent<SpriteRenderer>();

                    rendered_tile.sprite = path;
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

	    Dictionary<string, Sprite> entitySpriteDictionary = new Dictionary<string, Sprite>() {
            { "blank",  Resources.Load<Sprite>("Sprites/map_features/empty") }
	    };

	    IEnumerable<SoftwareTool> concat = tools.Concat(sentries);
        foreach(var software in concat) {
            if (!entitySpriteDictionary.ContainsKey(software.string_id)) {
                Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", software.string_id));
                entitySpriteDictionary.Add(software.string_id, sprite);
            }
        }

        Utility.UnityLog(entitySpriteDictionary.Count);

        rowSpacer = 0;
        for(int row = 0; row < entityRow.Count; row++) {
            float columnSpacer = 0;
            for(int column = 0; column < entityRow[1].Count; column++) {
                bool realSoftware;
                KeyValuePair<int, SoftwareTool> tool = GetEntityByCoords(row, column, out realSoftware);

                if (realSoftware) {
                    Sprite success;
                    if (!entitySpriteDictionary.TryGetValue(tool.Value.string_id, out success) || success == null) {
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

    KeyValuePair<int, SoftwareTool> GetEntityByCoords(int x, int y, out bool successful)
    {
        int id = entityRow[x][y];
        SoftwareTool success;
        successful = entityIDs.TryGetValue(id, out success);

        return new KeyValuePair<int, SoftwareTool>(id, success);
    }

    int GetGeometryByCoords(int x, int y) {
        return geometryRow[x][y];
    }
	
	// Update is called once per frame
	void Update () {
	}
}
