using System.Xml.Serialization;
using Level.Entity;
using UnityEngine;

namespace Level {
    [XmlType(TypeName = "spawnpoint")]
    public class SpawnPoint : MapItem {
        [XmlIgnore]
        public SoftwareTool software;
        [XmlIgnore]
        public static MapItem Spawn = new MapItem { name = "Spawn Point", description = "Attack vector entry point for your software tools", string_id = "spawnpoint", sprite = Resources.Load<Sprite>("Sprites/map_features/spawnpoint") };
    }
}
