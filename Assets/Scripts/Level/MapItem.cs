using System.Xml.Serialization;
using UnityEngine;
using Utility.Collections.Grid;

namespace Level {
    [XmlInclude(typeof(Pickup))]
    [XmlType(TypeName = "mapitem")]
    public class MapItem : IGridLocator{
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("description")]
        public string description;
        [XmlAttribute("string_id")]
        public string string_id;
        [XmlIgnore]
        protected Vector2 gridPosition;
        [XmlIgnore]
        public Sprite sprite;
        [XmlIgnore]
        public int id;

        [XmlIgnore]
        public static MapItem BlankTile = new MapItem {name = "An empty tile", description = "Unused memory address sequence.", string_id = "empty", sprite = Resources.Load<Sprite>("Sprites/map_features/empty")};
        [XmlIgnore]
        public static MapItem MapPath = new MapItem {name = "Map Piece", description = "Traversable network pathing", string_id = "path", sprite = Resources.Load<Sprite>("Sprites/map_features/path") };

        public MapItem() { }

        public MapItem(MapItem mi) {
            name = mi.name;
            description = mi.description;
            string_id = mi.string_id;
            gridPosition = mi.gridPosition;
            sprite = mi.sprite;
            id = mi.id;
        }

        public int ID {
            get { return id; }
            set { id = value; }
        }

        public void SetPosition(Vector2 pos) {
            gridPosition = pos;
        }

        public Vector2 GetPosition() {
            return gridPosition;
        }

        public void SetPosition(int x, int y) {
            SetPosition(new Vector2(x, y));
        }

        public override string ToString() {
            return string.Format("{0}, '{1}'", GetType().FullName, name);
        }
    }
}