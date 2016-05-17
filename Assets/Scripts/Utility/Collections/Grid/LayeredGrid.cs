using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.Collections.Grid {
    public class LayeredGrid<T> : IEnumerable<GridCollection<T>>, ILayeredGrid<T> where T : IGridLocator {
        private Dictionary<string, GridCollection<T>> layers = new Dictionary<string, GridCollection<T>>();
        private GridPiece<T> emptyPiece;
        private int? rows;
        private int? columns;

        public enum Layers {
            GEOMETRY,
            ENTITY,
        }

        public LayeredGrid(GridPiece<T> emptyPiece, int? rows = null, int? columns = null) {
            this.emptyPiece = emptyPiece;
            this.rows = rows;
            this.columns = columns;
        }

        public GridCollection<T> this[int i] {
            get { return layers.Values.ToArray()[i]; }
            set { layers.Values.ToArray()[i] = value; }
        }

        public GridCollection<T> Add(string lname) {
            GridCollection<T> layer = new GridCollection<T>(emptyPiece, rows, columns);
            layers.Add(lname, layer);
            return layer;
        }

        public void Remove(string lname) {
            layers.Remove(lname);
        }

        public GridCollection<T> GetLayer(Layers layerName) {
            String name = layerName.ToString().ToLower();
            return GetLayer(name);
        }

        public GridCollection<T> GetLayer(string lname) {
            GridCollection<T> layer;
            if (layers.TryGetValue(lname, out layer))
                return layer;
            throw new ArgumentException(string.Format("No layer was found named {0}", lname));
        }

        public GridPiece<T> GetLowestElement(int x, int y) {
            GridPiece<T> element = default(GridPiece<T>);

            foreach (var layer in layers.Values) {
                element = layer.Get(x, y);
                if (element != null && element.ID != 0)
                    break;
            }

            return element;
        }

        public GridPiece<T> GetLowestElement(Vector2 pos) {
            return GetLowestElement((int)pos.x, (int)pos.y);
        }

        public GridPiece<T> GetHighestElement(int x, int y) {
            GridPiece<T> element = default(GridPiece<T>);

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
