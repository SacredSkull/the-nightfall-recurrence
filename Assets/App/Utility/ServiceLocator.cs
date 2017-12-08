using System;
using Level;
using UnityUtilities.Collections.Grid;

namespace Utility {
    public static class ServiceLocator {
        public static ILayeredGrid<MapItem> GetLevelLayeredGrid() {
            throw new NotSupportedException("Create Trails via a TrailFactory so that LevelLayeredGrid can be injected!");
        }

        public static IGridCollection<MapItem> GetLevelEntityGrid() {
            throw new NotSupportedException("Create Trails via a TrailFactory so that LevelEntityGrid can be injected!");
        }
    }
}