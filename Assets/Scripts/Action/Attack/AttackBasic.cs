using System;
using System.Xml.Serialization;

public class AttackBasic : Attack {
    [XmlAttribute("damage")]
    public short damage;
    [XmlAttribute("range")]
    public short range;

    public override bool attack(SoftwareTool target, SoftwareTool source) {
        if(!(source.isEnemy) && !(target.isEnemy)) {
            return false;
        }
        if(source.isEnemy && target.isEnemy) {
            throw new Exception("Traitor sentry program detected!");
        }
        target.health -= damage;
        return true;
    }
}
