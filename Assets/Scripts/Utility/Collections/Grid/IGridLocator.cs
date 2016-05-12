using UnityEngine;

namespace Utility.Collections.Grid {
    public interface IGridLocator {
        void SetPosition(int x, int y);
        void SetPosition(Vector2 pos);

        Vector2 GetPosition();
    }
}