using System.Xml.Serialization;

public abstract class Attack {
    [XmlAttribute("name")]
    public string name;

    public abstract bool attack(SoftwareTool target, SoftwareTool source);
}
