using System.Collections.Generic;
using System.Xml.Serialization;
using Controllers;
using thelab.mvc;

namespace Level {
    [XmlType(TypeName = "tool")]
    public class FeaturesXMLRoot {
        [XmlElement(ElementName = "mapitem", Type = typeof(MapItem)),
         XmlElement(ElementName = "pickup", Type = typeof(Pickup)),
         XmlElement(ElementName = "spawnpoint", Type = typeof(SpawnPoint))]
        public List<MapItem> features;
    }
}
