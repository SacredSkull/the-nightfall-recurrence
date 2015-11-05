using System;
using System.Xml.Serialization;

[XmlType(TypeName = "pickup")]
public class Pickup : MapItem {
    [XmlElement("required")]
    public bool required;
}
