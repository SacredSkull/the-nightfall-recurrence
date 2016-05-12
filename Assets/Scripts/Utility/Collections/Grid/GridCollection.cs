using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.Collections.Grid {
    public class GridPiece<T> {
        public int ID;
        public T Value;
        public int Cost = 1;
    }

    public class GridCollection<T> : IEnumerable<T> where T : IGridLocator {
        private readonly Dictionary<int, Dictionary<int, GridPiece<T>>> gridDictionary;

        public GridCollection(int? rowCount = null, int? columnCount = null) {
            gridDictionary = new Dictionary<int, Dictionary<int, GridPiece<T>>>();

            if (rowCount != null && columnCount != null && rowCount > 0 && columnCount > 0) {
                for (int row = 0; row < rowCount; row++) {
                    gridDictionary[row] = new Dictionary<int, GridPiece<T>>();

                    for (int column = 0; column < columnCount; column++) {
                        gridDictionary[row][column] = new GridPiece<T>();
                    }
                }
            }
        }

        public GridPiece<T> Get(int x, int y) {
            Dictionary<int, GridPiece<T>> rowDic;
            gridDictionary.TryGetValue(x, out rowDic);
            if (rowDic == null)
                return default(GridPiece<T>);

            GridPiece<T> colT;
            rowDic.TryGetValue(y, out colT);

            return colT;
        }

        public GridPiece<T> Get(Vector2 vec) {
            return Get((int) vec.x, (int) vec.y);
        }

        public IEnumerable<GridPiece<T>> GetRow(int x) {
            return gridDictionary[x].Values;
        }

        public void SetRow(int x, IEnumerable<GridPiece<T>> values)
        {
            int y = 0;
            foreach (GridPiece<T> piece in values) {
                if(piece.Value != null)
                    piece.Value.SetPosition(x, y);
                gridDictionary[x][y++] = piece;
            }
        }

        public void Set(int x, int y, int id, T value) {
            // Make sure to instantiate the collections at both keys.
            if(!gridDictionary.ContainsKey(x))
                gridDictionary.Add(x, new Dictionary<int, GridPiece<T>>());
            if(!gridDictionary[x].ContainsKey(y))
                gridDictionary[x].Add(y, new GridPiece<T>());

            value.SetPosition(x, y);

            gridDictionary[x][y].Value = value;
            gridDictionary[x][y].ID = id;
        }

        public bool Contains(int x, int y) {
            return gridDictionary.ContainsKey(x) && gridDictionary[x].ContainsKey(y);
        }

        public IEnumerator<T> GetEnumerator() {
            return gridDictionary.SelectMany(x => x.Value).Select(x => x.Value).Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerable<GridPiece<T>> Values {
            get {
                return gridDictionary.SelectMany(x => x.Value).Select(x => x.Value);
            }
        }

        public int Area {
            get { return Width * Height; }
        }

        public int Width {
            get {
                Dictionary<int, GridPiece<T>> val;
                if (gridDictionary.TryGetValue(0, out val))
                    return val.Count;
                return 0;
            }
        }

        public int Height {
            get { return gridDictionary.Keys.Count; } 
        }
    }
}
