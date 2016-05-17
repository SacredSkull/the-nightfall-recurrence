using System.Collections.Generic;
using UnityEngine;

namespace Utility.Collections.Grid {
    public interface ILayeredGrid<T> where T : IGridLocator {
        GridCollection<T> this[int i] { get; set; }

        GridCollection<T> Add(string lname);
        IEnumerator<GridCollection<T>> GetEnumerator();
        GridPiece<T> GetHighestElement(Vector2 pos);
        GridPiece<T> GetHighestElement(int x, int y);
        GridCollection<T> GetLayer(string lname);
        GridCollection<T> GetLayer(LayeredGrid<T>.Layers lname);
        void Remove(string lname);
    }
}