using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


public abstract class Attackable {
    [XmlIgnore]
    public int health { get; set; }
    [XmlIgnore]
    public int movement { get; set; }
}
