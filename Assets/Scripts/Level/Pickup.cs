using System.Xml.Serialization;

namespace Level {
    [XmlType(TypeName = "pickup")]
    public class Pickup : MapItem {
        [XmlElement("required")]
        public bool required;
    }
}
