using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace Utility.Collections.Grid {
    public class LayeredGrid<T> : IEnumerable<GridCollection<T>> where T : IGridLocator {
        private Dictionary<string, GridCollection<T>> layers = new Dictionary<string, GridCollection<T>>();

        public GridCollection<T> this[int i] {
            get { return layers.Values.ToArray()[i]; }
            set { layers.Values.ToArray()[i] = value; }
        }

        public void Add(string lname, GridCollection<T> layer) {
            layers.Add(lname, layer);
        }

        public void Remove(string lname) {
            layers.Remove(lname);
        }

        public GridCollection<T> GetLayer(string lname) {
            GridCollection<T> layer;
            if (layers.TryGetValue(lname, out layer))
                return layer;
            throw new ArgumentException(string.Format("No layer was found named {0}", lname));
        }

        public GridPiece<T> GetHighestElement(int x, int y) {
            GridPiece<T> element = new GridPiece<T>();

            foreach (var layer in layers.Values.Reverse()) {
                element = layer.Get(x, y);
                if (element != null && element.ID != 0)
                    break;
            }

            return element;
        }

        public GridPiece<T> GetHighestElement(Vector2 pos) {
            return GetHighestElement((int)pos.x, (int)pos.y);
        }

        public IEnumerator<GridCollection<T>> GetEnumerator() {
            return layers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
