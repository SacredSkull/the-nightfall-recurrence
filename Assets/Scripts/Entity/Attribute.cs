using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


public class Attribute {
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("value")]
    public int value;
}
