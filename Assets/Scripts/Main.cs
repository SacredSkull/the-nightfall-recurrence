using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using SacredSkull.Software;

public class Main : MonoBehaviour {
    Map map;

	public bool dumpLevelXML;
	public bool dumpEntityXML;
	public bool dumpProgramXML;
	public bool dumpEnemyXML;
	public bool dumpGeometry;
	public bool dumpEntities;
	public bool dumpTileSets;
	public bool dumpEntityRasters;

    private Dictionary<int, softwareTool> entityIDs = new Dictionary<int, softwareTool>();
    private List<List<int>> geometryRow = new List<List<int>>();
    private List<List<int>> entityRow = new List<List<int>>();

	// Use this for initialization
	void Start () {
        XmlSerializer deserializerLevel = new XmlSerializer(typeof(Map));
        TextReader readerLevel = new StreamReader("./Assets/Levels/1.tmx");
        object objLevel = deserializerLevel.Deserialize(readerLevel);
        map = (Map)objLevel;
        readerLevel.Close();

		// This is incredibly useful for debugging XML parsing errors.
		// It will create an XML file based on what it has read from the original,
		// meaning that you can quickly spot discrepancies and fix them.
		if (Utility.level == LogLevels.DEBUG && dumpLevelXML) {
			XmlSerializer serializer = new XmlSerializer (typeof(Map)); 
			using (TextWriter writer = new StreamWriter(@"./XML_Level_Serialised.xml")) {
					serializer.Serialize (writer, map); 
			} 
		}

		//Sprite[] sprites = Resources.LoadAll<Sprite>();

        software tools = software.LoadFromFile(@"./Assets/Entities/Tools.xml");
        software sentries = software.LoadFromFile(@"./Assets/Entities/Sentries.xml");

        foreach(softwareTool tool in tools.tool){
            Utility.UnityLog(tool.string_id);
        }

        if (Utility.level == LogLevels.DEBUG && dumpProgramXML) {
            tools.SaveToFile(@"./XML_Tools_Serialised.xml");
            sentries.SaveToFile(@"./XML_Sentries_Serialised.xml");
        }

        

        Utility.UnityLog("Starting to parse Tilesets", LogLevels.INFO);

		//entityIDs.Add (0, "empty");

        foreach (TileSet ts in map.tilesets) {
			Utility.UnityLog("Currently working on the TS " + ts.name, LogLevels.DEBUG);
			// For some reason, Tiler doesn't store the first GID as the value actually written to 
			// file. Rather than rely on human based counting, let's use zero-indexed GIDs.
			int tileFirstGID = ts.firstgid;

			Utility.UnityLog("First GID of "+ ts.name +" is " + tileFirstGID.ToString(), LogLevels.DEBUG);
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
	                        Utility.UnityLog("Geo. ("+ pleasantRow.ToString() + ", " + (column+1).ToString()+ ") " + geometryRow[row][column].ToString(), LogLevels.DEBUG);
	                    }
	                }
				}
                

                Utility.UnityLog("Finished parsing Geometry layer", LogLevels.INFO);
            } else if (l.name == "Entities") {
                Utility.UnityLog("Starting to parse Entities layer", LogLevels.INFO);

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

				if(Utility.level == LogLevels.DEBUG && dumpEntities){
					Utility.UnityLog("Printing all Entitity values", LogLevels.DEBUG);
					for (int row = 0; row < entityRow.Count; row++) {
						int pleasantRow = row + 1;
						Utility.UnityLog("Ent. Row " + pleasantRow.ToString(), LogLevels.DEBUG);
						for (int column = 0; column < entityRow[row].Count; column++) {
							Utility.UnityLog("Ent. " + entityRow[row][column].ToString(), LogLevels.DEBUG);
						}
					}
				}

                Utility.UnityLog("Finished parsing Entity layer", LogLevels.INFO);
            }
        }

		Utility.UnityLog("Displaying loaded entities", LogLevels.INFO);
		if (dumpEntityRasters) {
			int tile = 1;
			for (int row = 0; row < entityRow.Count; row++) {
				for(int column = 0; column < entityRow[row].Count; column++){
					Utility.UnityLog(tile + ": " + entityIDs[entityRow[row][column]]);
					tile++;
				}
			}
		}

        Sprite path = Resources.Load<Sprite>("Sprites/map_features/wall");
        Sprite wall = Resources.Load<Sprite>("Sprites/map_features/wall");

        for (int row = 0; row < geometryRow.Count; row++)
        {
            for (int column = 0; column < geometryRow[1].Count; column++)
            {
                Utility.UnityLog("(" + (row+1).ToString() + "," + (column+1) + ") id is "+ GetEntityByCoords(row, column));



                SpriteRenderer rendered_tile;
                rendered_tile = new GameObject().AddComponent<SpriteRenderer>();

                rendered_tile.sprite = path;
                rendered_tile.name = "Grid Path";
                rendered_tile.transform.localScale += new Vector3(2f, 2f, 2f);
            }
        }

	}

    string GetEntityByCoords(int x, int y)
    {
        int id = geometryRow[x][y];
        return entityIDs[id];
    }
	
	// Update is called once per frame
	void Update () {
	}
}
