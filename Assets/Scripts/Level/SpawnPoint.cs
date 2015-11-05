using System.Xml.Serialization;

[XmlType(TypeName = "spawnpoint")]
public class SpawnPoint : MapItem {
    [XmlIgnore]
    public SoftwareTool software;
}
