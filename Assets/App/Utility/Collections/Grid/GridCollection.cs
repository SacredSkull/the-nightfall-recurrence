using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.Collections.Grid {
    public class GridCollection<T> : IEnumerable<T>, IGridCollection<T> where T : IGridLocator {
        private readonly Dictionary<int, Dictionary<int, GridPiece<T>>> gridDictionary;
        private readonly GridPiece<T> emptyPiece;
       

        public GridCollection(GridPiece<T> emptyPiece, int? rowCount = null, int? columnCount = null) {
            gridDictionary = new Dictionary<int, Dictionary<int, GridPiece<T>>>();
            this.emptyPiece = emptyPiece;

            if (rowCount != null && columnCount != null && rowCount > 0 && columnCount > 0) {
                for (int row = 0; row < rowCount; row++) {
                    gridDictionary[row] = new Dictionary<int, GridPiece<T>>();

                    for (int column = 0; column < columnCount; column++) {
                        gridDictionary[row][column] = new GridPiece<T>(new Vector2(row, column));
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

        public void Clear(Vector2 pos) {
            Set(pos, emptyPiece.ID, emptyPiece.Value);
        }

        public void Clear(IGridLocator obj) {
            Clear(obj.GetPosition());
        }

        public void ClearAll() {
            foreach (GridPiece<T> piece in this.Values) {
                Clear(piece.Position);
            }
        }

        public void Clear(int x, int y) {
            Clear(new Vector2(x, y));
        }

        public void SetRow(int x, IEnumerable<GridPiece<T>> values) {
            int y = 0;
            foreach (GridPiece<T> piece in values) {
                if (piece.Value == null && emptyPiece != null)
                    piece.Value = emptyPiece.Value;

                Set(new Vector2(x, y++), piece);
            }
        }

        public void Set(Vector2 pos, int id, T value) {
            Set((int)pos.x, (int)pos.y, id, value);
        }

        public void Set(int x, int y, int id, T value) {
            // Make sure to instantiate the collections at both keys.
            if(!gridDictionary.ContainsKey(x))
                gridDictionary.Add(x, new Dictionary<int, GridPiece<T>>());

            value.PositionSetEvent -= Move;
            value.PositionSetEvent += Move;

            value.DeletionEvent -= Clear;
            value.DeletionEvent += Clear;

            gridDictionary[x][y].ID = id;
            gridDictionary[x][y].Value = value;

            PieceChanged?.Invoke(new GridCollectionEventArgs<T>(gridDictionary[x][y]));
        }

        public void Set(Vector2 pos, GridPiece<T> piece) {
            Set(pos, piece.ID, piece.Value);
        }

        public void Move(Vector2 currentPos, Vector2 newPos, bool clear = true) {
            // Now copy our stored piece into the new position (overwriting).
            Set(newPos, Get(currentPos));

            // Set the initial position to empty.
            if(clear)
                Clear(currentPos);
        }

        public void Move(IGridLocator obj, Vector2 newPos, bool clear = true) {
            T casted;
            if(obj is T)
                casted = (T)obj;
            else {
                return;
            }

            Set(newPos, 0, casted);

            if(clear)
                Clear(obj.PreviousPosition);
        }

        public void Move(int x1, int y1, int x2, int y2, bool clear = true) {
            Move(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        public void Swap(Vector2 first, Vector2 second) {
            if(first == second) return;

            T valueCopy = Get(first).Value;
            int idCopy = Get(first).ID;

            Set(first, Get(second));
            Set(second, idCopy, valueCopy);
        }

        public void Swap(int x1, int y1, int x2, int y2) {
            Swap(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        public bool Contains(int x, int y) {
            return gridDictionary.ContainsKey(x) && gridDictionary[x].ContainsKey(y);
        }

        public bool Contains(Vector2 vect) {
            return Contains((int)vect.x, (int)vect.y);
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

        public event PieceChangedHandler<T> PieceChanged;
        public int Area => Width * Height;

        public int Width {
            get {
                Dictionary<int, GridPiece<T>> val;
                if (gridDictionary.TryGetValue(0, out val))
                    return val.Count;
                return 0;
            }
        }

        public int Height => gridDictionary.Keys.Count;
    }
}
