using System.Collections.Generic;
using UnityEngine;

namespace Utility.Collections.Grid {
    public interface ILayeredGrid<T> where T : IGridLocator {
        GridCollection<T> this[int i] { get; set; }

        GridCollection<T> Add(string lname);
        IEnumerator<GridCollection<T>> GetEnumerator();
        KeyValuePair<string, GridPiece<T>> GetHighestElement(Vector2 pos, string startingLayer = null);
        KeyValuePair<string, GridPiece<T>> GetHighestElement(int x, int y, string startingLayer = null);
        
        KeyValuePair<string, GridPiece<T>> GetLowestElement(Vector2 pos, string startingLayer = null);
        KeyValuePair<string, GridPiece<T>> GetLowestElement(int x, int y, string startingLayer = null);
        
        GridCollection<T> GetLayer(string lname);
        void Remove(string lname);
    }
}