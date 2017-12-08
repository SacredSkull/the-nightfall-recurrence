using System;
using System.Xml.Serialization;
using Level.Entity;

namespace Action.Ability {
    public class MapModifier : Attack {
        public override bool Execute(SoftwareTool target, SoftwareTool source) {
            return false;
        }
        [XmlAttribute]
        public string value;
        public void Attack(int x, int y, SoftwareTool source) {
            throw new NotImplementedException();
        }
    }
}
