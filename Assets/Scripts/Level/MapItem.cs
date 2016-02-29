using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(Pickup))]
[XmlType(TypeName = "mapitem")]
public class MapItem {
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

    public virtual MapItem Copy(MapItem mi) {
        MapItem newMI = new MapItem {
            name = mi.name,
            description = mi.description,
            string_id = mi.string_id,
            gameobject = null,
            gridPosition = new Vector2(),
            sprite = null
        };

        return newMI;
    }
}
