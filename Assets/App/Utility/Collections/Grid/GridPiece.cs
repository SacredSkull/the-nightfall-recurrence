using System;
using UnityEngine;

namespace Utility.Collections.Grid {
    public class GridPiece<T> : IComparable<GridPiece<T>> where T : IGridLocator {
        public int ID;
        public T Value;
        public int Cost = 1;
        public GameObject GameObject;
        public Vector2 Position;

        public GridPiece() {}

        public GridPiece(int ID, T val, Vector2 pos, int cost, GameObject go) {
            this.ID = ID;
            Value = val;
            Position = pos;
            Cost = cost;
            GameObject = go;
        }

        public GridPiece(Vector2 pos) {
            Position = pos;
        }

        public int CompareTo(GridPiece<T> other) {
            if (this.Position.x > other.Position.x)
                return 1;
            if (this.Position.x < other.Position.x)
                return -1;

            if (this.Position.y > other.Position.y)
                return 1;
            if (this.Position.y < other.Position.y)
                return - 1;

            return 0;
        }

        public override string ToString() {
            return string.Format("GridPiece<{0}> ID: {1}, Value: {2}", typeof(T).FullName, ID, Value.ToString());
        }
    }
}