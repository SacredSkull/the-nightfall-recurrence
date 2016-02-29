using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Scripts.Action.Move;
using UnityEngine;
using Object = UnityEngine.Object;

[XmlInclude(typeof(Pickup))]
[XmlType(TypeName = "mapitem")]
public class MapItem : IGridTileIdentifier{
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("description")]
    public string description;
    [XmlAttribute("string_id")]
    public string string_id;
    [XmlIgnore]
    public GameObject gameobject;
    [XmlIgnore]
    public Vector2 gridPosition;
    [XmlIgnore]
    public Sprite sprite;
    [XmlIgnore]
    public int id;

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

    protected static T Clone<T>(T source) {
        if(!typeof(T).IsSerializable) {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if(System.Object.ReferenceEquals(source, null)) {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using(stream) {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }

    public int ID {
        get
        {
            return this.id;
        }
        set;
    }
}
