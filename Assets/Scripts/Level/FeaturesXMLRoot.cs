using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

[XmlType(TypeName = "tool")]
public class FeaturesXMLRoot {
    [XmlElement(ElementName = "mapitem", Type = typeof(MapItem)),
        XmlElement(ElementName = "pickup", Type = typeof(Pickup)),
        XmlElement(ElementName = "spawnpoint", Type = typeof(SpawnPoint))]
    public List<MapItem> features;
}
