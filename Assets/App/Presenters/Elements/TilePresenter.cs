using Editor;
using Karma.Metadata;
using Level;
using UnityEngine;
using Zenject;

namespace Presenters {
    
    
    [Element(PrefabPath)]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TilePresenter : MVCPresenter2D {
        public class TileData : ScriptableObject {
            [ReadOnly]
            public Vector2 Position;
            [ReadOnly]
            public string Name;
            [ReadOnly]
            public string Description;
            [ReadOnly]
            public string SpriteName;
            [ReadOnly]
            public string Type;
            
            public void SetData(MapItem mi) {
                if (mi == null)
                    return;

                Name = mi.name;
                Description = mi.description;
                SpriteName = mi.string_id;
                Position = mi.GetPosition();
                Type = mi.GetType().Name;
            }
        }
        
        //public class Factory : Factory<Sprite, MapItem, TilePresenter> { }
        public const string PrefabPath = "Grid/GridPiece";

        private MapItem _mapItem = null;
        [HideInInspector]
        public MapItem MapItem {
            get { return _mapItem; }
            set {
                _mapItem = value;
                GetComponent<SpriteRenderer>().sprite = value.sprite;
            }
        }

        public delegate void HoverEventHandler(TilePresenter tile, GameObject obj);
        public event HoverEventHandler HoverEvent;

        public delegate void SingleClientEventHandler(TilePresenter tile, GameObject obj);
        public event SingleClientEventHandler SingleClickEvent;

        [SerializeField]
        public TileData tileData;
        private static bool hovering;
        private static bool clicking;
        private static int Counter = 0;

        private void Awake() {
            tileData = ScriptableObject.CreateInstance<TileData>();
            tileData.SetData(MapItem);

            HoverEvent += (tile, obj) => {
                //Logger.UnityLog($"Hover on {tile.MapItem.name}");
                hovering = true;
            };

//            MapItem.OnValueChange += () => {
//                GetComponent<SpriteRenderer>().sprite = MapItem.Value.sprite;               
//            };
        }

        public override void OnMouseEnter() {
            // !hovering ?! Ensures the event fires only once per hover. 
            if (!hovering)
                HoverEvent?.Invoke(this, gameObject);
        }

        public override void OnMouseExit() {
            hovering = false;
        }

        public override void OnMouseUp() {
            SingleClickEvent?.Invoke(this, gameObject);
            Counter++;
        }

        public class Factory : Factory<TilePresenter> { }
    }
}
