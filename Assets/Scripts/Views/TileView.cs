using System.Collections;
using Level;
using UnityEngine;

namespace Views {
    class TileView : View {
        public MapItem MapItem;
        public Sprite Sprite;

        public delegate void HoverEventHandler(TileView tile, GameObject obj);
        public static event HoverEventHandler HoverEvent;

        public delegate void SelectEventHandler(TileView tile, GameObject obj);
        public static event SelectEventHandler SelectEvent;

        private static bool hovering;
        private static bool clicking;

        private void Awake() {
            HoverEvent += (tile, obj) => {
                //Logger.UnityLog($"Hover on {tile.MapItem.name}");
                hovering = true;
            };
        }

        public override void MouseEnter() {
            if (!hovering)
                HoverEvent?.Invoke(this, gameObject);
        }

        public override void MouseExit() {
            hovering = false;
        }

        public override void MouseDown() {
            SelectEvent?.Invoke(this, gameObject);
        }
    }
}
