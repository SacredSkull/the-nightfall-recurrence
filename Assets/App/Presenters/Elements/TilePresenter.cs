using Editor;
using Karma.Metadata;
using Level;
using UnityEngine;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

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
        [Inject]
        public static ILogger Logger;
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

        public void ForceSprite(Sprite sprite = null) {
            GetComponent<SpriteRenderer>().sprite = sprite ?? MapItem.BlankTile.sprite;
        }

        public delegate void HoverEventHandler(TilePresenter tile, GameObject obj);
        public event HoverEventHandler HoverEvent;
        
        public delegate void UnhoverEventHandler(TilePresenter tile, GameObject obj);
        public event UnhoverEventHandler UnhoverEvent;

        public delegate void SingleClickEventHandler(TilePresenter tile, GameObject obj);
        public event SingleClickEventHandler SingleClickEvent;

        [SerializeField]
        public TileData tileData;
        private static bool hovering;
        private static bool clicking;
        private static int Counter = 0;
        private static Vector2 currentHoverTarget = new Vector2(-1, -1);

        private void Awake() {
            tileData = ScriptableObject.CreateInstance<TileData>();
            tileData.SetData(MapItem);

            HoverEvent += (tile, obj) => {
                hovering = true;
            };
        }

        public void ClearSingleClickHandlers() {
            SingleClickEvent = null;
        }
        
        public void ClearHoverHandlers() {
            HoverEvent = null;
            UnhoverEvent = null;
        }

        public override void OnMouseEnter() {
            // !hovering ?! Ensures the event fires only once per hover. 
            if (!hovering)
                HoverEvent?.Invoke(this, gameObject);
        }

        public override void OnMouseExit() {
            hovering = false;
            UnhoverEvent?.Invoke(this, gameObject);
        }

        public override void OnMouseUp() {
            SingleClickEvent?.Invoke(this, gameObject);
            Counter++;
        }

        public class Factory : Factory<TilePresenter> { }
    }
}
