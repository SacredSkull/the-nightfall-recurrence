using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Collections {
    public class GridPiece<T>
    {
        public int id;
        public T piece;
    }

    public class GridCollection<T> : IEnumerable<T>
    {
        private readonly Dictionary<int, Dictionary<int, GridPiece<T>>> gridDictionary;

        public GridCollection() {
            gridDictionary = new Dictionary<int, Dictionary<int, GridPiece<T>>>();
        }

        public GridPiece<T> Get(int x, int y) {
            Dictionary<int, GridPiece<T>> rowDic;
            gridDictionary.TryGetValue(x, out rowDic);
            if(rowDic == null)
                throw new KeyNotFoundException(string.Format("Could not find grid row/X number {0}", x));

            GridPiece<T> success;
            rowDic.TryGetValue(y, out success);

            if(rowDic == null)
                throw new KeyNotFoundException(string.Format("Could not find grid column/Y number {0}", y));

            return success;
        }

        public void Set(int x, int y, int id, T value) {
            gridDictionary[x][y].piece = value;
            gridDictionary[x][y].id = id;
        }

        public bool Contains(int x, int y)
        {
            if (!gridDictionary.ContainsKey(x))
                return false;
            if (!gridDictionary[x].ContainsKey(y))
                return false;
            return true;
        }

        public IEnumerator<T> GetEnumerator() {
            return gridDictionary.SelectMany(x => x.Value).Select(x => x.Value).Select(x => x.piece).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerable<T> Values {
            get
            {
                return gridDictionary.SelectMany(x => x.Value).Select(x => x.Value).Select(x => x.piece);
            }
        }

        public int Count {
            get
            {
                int rows = gridDictionary.Keys.Count;
                int columns = gridDictionary.Values.Count;
                return rows * columns;
            }
        }

        public int Width
        {
            get { return gridDictionary.Keys.Count; }
        }

        public int Height
        {
            get { return gridDictionary.Values.Count; }
        }
    }
}
