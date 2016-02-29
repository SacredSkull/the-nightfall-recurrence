using System;
using System.Xml.Serialization;

public class MapModifier : Attack {
    public override bool attack(SoftwareTool target, SoftwareTool source) {
        return false;
    }
    [XmlAttribute]
    public string value;
    public void Attack(int x, int y, Attackable source) {
        throw new NotImplementedException();
    }
}
