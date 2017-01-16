using UnityEngine;

namespace Utility.Collections.Grid {
    public delegate void PositionSetHandler(IGridLocator obj, Vector2 newPos, bool clearPrevious = true);
    public delegate void DeletionHandler(IGridLocator obj);
    public interface IGridLocator {
        event PositionSetHandler PositionSetEvent;
        event DeletionHandler DeletionEvent;

        void SetPosition(int x, int y, bool clearPrevious = true);
        void SetPosition(Vector2 pos, bool clearPrevious = true);

        Vector2 PreviousPosition { get; }

        Vector2 GetPosition();
    }
}