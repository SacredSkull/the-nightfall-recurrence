using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Clifton.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Utility.Collections.Grid {
    public class LayeredGrid<T> : IEnumerable<GridCollection<T>>, ILayeredGrid<T> where T : IGridLocator {
        // Got this gem from (http://www.codeproject.com/KB/recipes/GenericKeyedList.aspx)[here].
        private RecencyOrderedDictionary<string, GridCollection<T>> layers = new RecencyOrderedDictionary<string, GridCollection<T>>();
        private GridPiece<T> emptyPiece;
        private int? rows;
        private int? columns;

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

        public GridCollection<T> GetLayer(string lname) {
            GridCollection<T> layer;
            if (layers.TryGetValue(lname, out layer))
                return layer;
            throw new ArgumentException($"No layer was found named {lname}");
        }

        public KeyValuePair<string, GridPiece<T>> GetLowestElement(int x, int y, string startingLayer = null) {
            GridPiece<T> element = default(GridPiece<T>);
            bool matchedLayer = startingLayer == null;
            string layerName = "";
            
            if(startingLayer != null && !layers.ContainsKey(startingLayer))
                throw new ArgumentException("Your starting layer should probably exist in the collection.", startingLayer);

            foreach (KeyValuePair<string, GridCollection<T>> layer in layers) {
                // The starting layer is included.
                // Essentially we can find the first layer in anything below this layer name.
                if (!matchedLayer) {
                    if (layer.Key == startingLayer)
                        matchedLayer = true;
                    else
                        continue;
                }
                layerName = layer.Key;
                element = layer.Value.Get(x, y);
                if (element != null && element.ID != 0)
                    break;
            }

            return new KeyValuePair<string, GridPiece<T>>(layerName, element);
        }

        public KeyValuePair<string, GridPiece<T>> GetLowestElement(Vector2 pos, string startingLayer = null) {
            return GetLowestElement((int)pos.x, (int)pos.y, startingLayer);
        }

        public KeyValuePair<string, GridPiece<T>> GetHighestElement(int x, int y, string startingLayer = null) {
            GridPiece<T> element = default(GridPiece<T>);
            bool matchedLayer = startingLayer == null;
            string layerName = "";
            
            if(startingLayer != null && !layers.ContainsKey(startingLayer))
                throw new ArgumentException("Your starting layer should probably exist in the collection.", startingLayer);
            
            foreach (KeyValuePair<string, GridCollection<T>> layer in layers.Reverse()) {
                // The starting layer is included.
                // Essentially we can find the highest layer in anything below this layer name.
                if (!matchedLayer) {
                    if (layer.Key == startingLayer)
                        matchedLayer = true;
                    else
                        continue;
                }
                layerName = layer.Key;
                element = layer.Value.Get(x, y);
                if (element != null && element.ID != 0)
                    break;
            }

            return new KeyValuePair<string, GridPiece<T>>(layerName, element);
        }

        public KeyValuePair<string, GridPiece<T>> GetHighestElement(Vector2 pos, string startingLayer = null) {
            return GetHighestElement((int)pos.x, (int)pos.y, startingLayer);
        }

        public IEnumerator<GridCollection<T>> GetEnumerator() {
            return layers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
