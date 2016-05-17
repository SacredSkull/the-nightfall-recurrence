using System.Xml.Serialization;

namespace Action.Attack {
    public class Attribute {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("value")]
        public int value;
    }
}
