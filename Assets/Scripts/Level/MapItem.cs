using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using Utility;
using Utility.Collections;
using Utility.Collections.Grid;

[XmlInclude(typeof(Pickup))]
[XmlType(TypeName = "mapitem")]
public class MapItem : IGridLocator{
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("description")]
    public string description;
    [XmlAttribute("string_id")]
    public string string_id;
    [XmlIgnore]
    public GameObject gameobject;
    [XmlIgnore]
    protected Vector2 gridPosition;
    [XmlIgnore]
    public Sprite sprite;
    [XmlIgnore]
    public int id;

    [XmlIgnore]
    public static MapItem BlankTile = new MapItem {name = "An empty tile", description = "Unused memory address sequence.", string_id = "empty", sprite = Resources.Load<Sprite>("Sprites/map_features/empty")};
    [XmlIgnore]
    public static MapItem MapPath = new MapItem {name = "Map Piece", description = "Traversable network pathing", string_id = "path", sprite = Resources.Load<Sprite>("Sprites/map_features/path") };

    public MapItem() {
    }

    public MapItem(MapItem mi) {
        name = mi.name;
        description = mi.description;
        string_id = mi.string_id;
        gameobject = null;
        gridPosition = new Vector2();
        sprite = mi.sprite;
    }

//    protected static T Clone<T>(T source) {
//        if(!typeof(T).IsSerializable) {
//            throw new ArgumentException("The type must be serializable.", "source");
//        }
//
//        // Don't serialize a null object, simply return the default for that object
//        if(ReferenceEquals(source, null)) {
//            return default(T);
//        }
//
//        IFormatter formatter = new BinaryFormatter();
//        Stream stream = new MemoryStream();
//        using(stream) {
//            formatter.Serialize(stream, source);
//            stream.Seek(0, SeekOrigin.Begin);
//            return (T)formatter.Deserialize(stream);
//        }
//    }

    public int ID {
        get
        {
            return this.id;
        }
        set { id = value; }
    }

    public void SetPosition(Vector2 pos) {
        this.gridPosition = pos;
    }

    public Vector2 GetPosition() {
        return gridPosition;
    }

    public void SetPosition(int x, int y) {
        SetPosition(new Vector2(x, y));
    }
}