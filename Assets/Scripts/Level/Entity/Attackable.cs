using System;
using System.Xml.Serialization;

public abstract class Attackable : MapItem {
    [XmlIgnore]
    public int health { get; set; }
    [XmlIgnore]
    public int movement { get; set; }
}
