using System.Xml.Serialization;

public class Attribute {
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("value")]
    public int value;
}
