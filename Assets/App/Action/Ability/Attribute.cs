using System.Xml.Serialization;

namespace Action.Ability {
    public class Attribute {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("operator")]
        public string operation;
        [XmlAttribute("value")]
        public int value;
        [XmlAttribute("turns")]
        public int turnCount;
    }
}
