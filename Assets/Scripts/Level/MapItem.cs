using System.Xml.Serialization;
using UnityEngine;
using Utility.Collections.Grid;

namespace Level {
    [XmlInclude(typeof(Pickup))]
    [XmlType(TypeName = "mapitem")]
    public class MapItem : IGridLocator{
        public event PositionSetHandler PositionSetEvent;
        public event DeletionHandler DeletionEvent;

        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("description")]
        public string description;
        [XmlAttribute("string_id")]
        public string string_id;
        [XmlIgnore]
        protected Vector2 gridPosition;

        [XmlIgnore]
        public Vector2 PreviousPosition { get; protected set; }
        [XmlIgnore]
        public Sprite sprite;

        [XmlIgnore]
        public static MapItem BlankTile = new MapItem {
            name = "An empty tile",
            description = "Unused memory address sequence.",
            string_id = "empty",
            sprite = null
        };
        [XmlIgnore]
        public static MapItem MapPath = new MapItem {
            name = "Map Piece",
            description = "Traversable network pathing",
            string_id = "path",
            sprite = null
        };

        public MapItem() { }

        public MapItem(MapItem mi) {
            name = mi.name;
            description = mi.description;
            string_id = mi.string_id;
            gridPosition = mi.gridPosition;
            sprite = mi.sprite;
        }

        public void SetPosition(Vector2 pos, bool clearPrevious = true) {
            PreviousPosition = gridPosition;
            gridPosition = pos;
            PositionSetEvent?.Invoke(this, pos, clearPrevious);
        }

        public Vector2 GetPosition() {
            return gridPosition;
        }

        public void SetPosition(int x, int y, bool triggerEvent = true) {
            SetPosition(new Vector2(x, y), triggerEvent);
        }

        public override string ToString() {
            return string.Format("{0}, '{1}'", GetType().FullName, name);
        }

        public virtual void Delete() {
            DeletionEvent?.Invoke(this);
        }
    }
}