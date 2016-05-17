using System.Xml.Serialization;

namespace Level {
    public abstract class Attackable : MapItem {
        [XmlIgnore]
        public int health { get; set; }
        [XmlIgnore]
        public int movement { get; set; }
    }
}
