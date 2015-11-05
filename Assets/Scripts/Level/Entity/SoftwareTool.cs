using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType(TypeName = "tool")]
public class SoftwareTool : Attackable {
    [XmlAttribute("maxsize")]
    public short maxsize;
    [XmlAttribute("cost")]
    public short cost;
    [XmlAttribute("level")]
    public short level;
    [XmlAttribute("governor")]
    public string governor;
    [XmlIgnore]
    public bool isEnemy { get; set; }

    [XmlArray("attacks")]
    [XmlArrayItem(ElementName = "attackbasic", Type = typeof(AttackBasic)),
        XmlArrayItem(ElementName = "attributemodifier", Type = typeof(AttributeModifier)),
        XmlArrayItem(ElementName = "mapmodifier", Type = typeof(MapModifier))]
    public List<Attack> attacks { get; set; }
}