using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using Zenject;

namespace Level {
    [XmlInclude(typeof(Pickup))]
    [XmlType(TypeName = "mapitem")]
    [Serializable]
    public class MapItem : IGridLocator, IEquatable<MapItem> {
        // These events are for specific use (e.g. one-off tracking of a specific unit) - grid updates are handled by the GridPresenter, NOT here.
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
        public static readonly MapItem BlankTile = new MapItem {
            name = "An empty tile",
            description = "Unused memory address sequence.",
            string_id = "empty",
            sprite = null
        };
        
        [XmlIgnore]
        public static readonly MapItem MapPath = new MapItem {
            name = "Map Piece",
            description = "Traversable network pathing",
            string_id = "path",
            sprite = null
        };

        public MapItem() { }

        [Inject]
        public MapItem(MapItem mi) {
            name = mi.name;
            description = mi.description;
            string_id = mi.string_id;
            gridPosition = mi.gridPosition;
            sprite = mi.sprite;
        }

        public void SetPosition(Vector2 pos, bool clearPrevious = true, bool fireEvent = true) {
            PreviousPosition = gridPosition;
            gridPosition = pos;
            if(fireEvent)
                PositionSetEvent?.Invoke(this, pos, clearPrevious);
        }

        public Vector2 GetPosition() {
            return gridPosition;
        }

        public void SetPosition(int x, int y, bool clearPrevious = true, bool triggerEvent = true) {
            SetPosition(new Vector2(x, y), clearPrevious, triggerEvent);
        }

        public override string ToString() {
            return $"{GetType().FullName}, '{name}', @{GetPosition()}";
        }

        public virtual void Delete() {
            DeletionEvent?.Invoke(this);
        }

        public bool Equals(MapItem other) {
            if (ReferenceEquals(null, other))
                return false;
            return ReferenceEquals(this, other) || (string.Equals(string_id, other.string_id) && 
                                                    GetPosition() == other.GetPosition());
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((MapItem) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (description != null ? description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (string_id != null ? string_id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ gridPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ (sprite != null ? sprite.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PreviousPosition.GetHashCode();
                return hashCode;
            }
        }
    }
}