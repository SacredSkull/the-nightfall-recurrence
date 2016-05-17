using System;
using Level;
using Utility.Collections.Grid;

namespace Utility {
    public static class ServiceLocator {
        private static ILayeredGrid<MapItem> levelLayeredGrid;
        private static IGridGraph levelGraph;

        public static void ProvideLevelLayeredGrid(ILayeredGrid<MapItem> layeredGrid) {
            levelLayeredGrid = layeredGrid;
        }

        public static ILayeredGrid<MapItem> GetLevelLayeredGrid() {
            if (levelLayeredGrid == null)
                throw new NullReferenceException("The level's layered grid is set to a null reference.");
            return levelLayeredGrid;
        }

        public static void ProvideGraph(IGridGraph graph) {
            levelGraph = graph;
        }

        public static IGridGraph GetLevelGraph() {
            return levelGraph;
        }

        public static IGridCollection<MapItem> GetLevelGeometryGrid() {
            return GetLevelLayeredGrid().GetLayer(LayeredGrid<MapItem>.Layers.GEOMETRY);
        }

        public static IGridCollection<MapItem> GetLevelEntityGrid() {
            return GetLevelLayeredGrid().GetLayer(LayeredGrid<MapItem>.Layers.ENTITY);
        }
    }
}