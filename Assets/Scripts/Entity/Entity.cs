using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Entities")]
public class EntityList {
    [XmlElement("Entity")]
    public List<Entity> list = new List<Entity>();
}

public class Entity {
    public string name;
    public string description;
    public string sprite;
}