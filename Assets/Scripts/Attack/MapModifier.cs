using System;
using System.Xml.Serialization;

public class MapModifier : Attack {
    [XmlAttribute]
    public string value;
    public void Attack(int x, int y, Attackable source) {
        throw new NotImplementedException();
    }
}
