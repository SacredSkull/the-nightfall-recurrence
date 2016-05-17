using System.Collections.Generic;
using UnityEngine;

namespace Utility.Collections.Grid {
    public interface IGridGraph {
        int Cost(Vector2 vect);
        int Cost(int x, int y);
        int CountNodes();
        bool isPathable(Vector2 coord);
        bool isPathable(int x, int y);
        List<Vector2> Neighbours(Vector2 positionVector);
    }
}