using System.Collections.Generic;
using UnityEngine;

namespace Utility.Collections.Grid {
    public interface IGridCollection<T> where T : IGridLocator {
        int Area { get; }
        int Height { get; }
        int Width { get; }

        IEnumerable<GridPiece<T>> Values { get; }

        bool Contains(Vector2 vect);
        bool Contains(int x, int y);

        GridPiece<T> Get(Vector2 vec);
        GridPiece<T> Get(int x, int y);

        IEnumerator<T> GetEnumerator();
        IEnumerable<GridPiece<T>> GetRow(int x);

        void Move(Vector2 currentPos, Vector2 newPos);
        void Move(int x1, int y1, int x2, int y2);

        void Set(Vector2 pos, int id, T value);
        void Set(int x, int y, int id, T value);

        void Clear(int x, int y);
        void Clear(Vector2 pos);

        void SetRow(int x, IEnumerable<GridPiece<T>> values);

        void Swap(Vector2 first, Vector2 second);
        void Swap(int x1, int y1, int x2, int y2);
    }
}