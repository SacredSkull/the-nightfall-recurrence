using System.Xml.Serialization;

[XmlInclude(typeof(Pickup))]
[XmlType(TypeName = "mapitem")]
public class MapItem {
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("description")]
    public string description;
    [XmlAttribute("string_id")]
    public string string_id;
}
