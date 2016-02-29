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
    public Vector2 position;
    [XmlIgnore]
    public Sprite sprite;
}
