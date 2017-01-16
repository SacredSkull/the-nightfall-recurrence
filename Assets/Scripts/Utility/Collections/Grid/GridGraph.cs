using System;
using System.Collections.Generic;
using System.Linq;
using Level.Entity;
using UnityEngine;

namespace Utility.Collections.Grid {
    public interface INodeValidator {
        bool isPathable(int x, int y);
        bool isPathable(Vector2 coord);
    }

    public class GridGraph<T> : INodeValidator, IGridGraph where T : IGridLocator {
        readonly Vector2 DIR_DOWN = new Vector2(-1, 0);
        readonly Vector2 DIR_UP = new Vector2(1, 0);
        readonly Vector2 DIR_LEFT = new Vector2(0, -1);
        readonly Vector2 DIR_RIGHT = new Vector2(0, 1);

        public LayeredGrid<T> LayeredGrid;
        private GridCollection<T> GeometryGrid;
        private GridCollection<T> EntityGrid;
        private HashSet<int> ValidPathIDs;
        private HashSet<int> ValidEntityIDs;

        public GridGraph(LayeredGrid<T> layered, HashSet<int> validPathIDs, HashSet<int> validEntityIDs) {
            LayeredGrid = layered;
            GeometryGrid = layered.GetLayer("geometry");
            EntityGrid = layered.GetLayer("entity");
            ValidPathIDs = validPathIDs;
            ValidEntityIDs = validEntityIDs;

            if (GeometryGrid == null)
                throw new ArgumentNullException("layered", "The geometry grid is null");
            if (EntityGrid == null)
                throw new ArgumentNullException("layered", "The entity grid is null");
        }

        public int CountNodes() {
            return (int)LayeredGrid.Average(x => x.Area);
        }

        public bool isPathable(int x, int y) {
            GridPiece<T> geomePiece = GeometryGrid.Get(x, y);
            GridPiece<T> entPiece = EntityGrid.Get(x, y);

            if (geomePiece == null || entPiece == null)
                return false;

            if(entPiece.Value is SoftwareTool || entPiece.Value is TrailTile)
                return false;

            return ValidPathIDs.Contains(geomePiece.ID);
        }

        public bool isPathable(Vector2 coord) {
            return isPathable((int)coord.x, (int)coord.y);
        }

        public int Cost(Vector2 vect) {
            return GeometryGrid.Get(vect).Cost;
        }

        public int Cost(int x, int y) {
            return Cost(new Vector2(x, y));
        }

        public List<Vector2> Neighbours(Vector2 positionVector, Vector2[] WhitelistedCoords) {
            int x = (int)positionVector.x;
            int y = (int)positionVector.y;

            List<Vector2> edges = new List<Vector2>();

            if(WhitelistedCoords == null)
                WhitelistedCoords = new Vector2[0];

            // Left
            if (GeometryGrid.Contains(positionVector + DIR_LEFT) && (WhitelistedCoords.Contains(positionVector - DIR_RIGHT) || isPathable(positionVector + DIR_LEFT))) {
                edges.Add(positionVector + DIR_LEFT);
            }

            // Up
            if (GeometryGrid.Contains(positionVector + DIR_UP) && (WhitelistedCoords.Contains(positionVector - DIR_DOWN) || isPathable(positionVector + DIR_UP))) {
                edges.Add(positionVector + DIR_UP);
            }

            // Down
            if (GeometryGrid.Contains(positionVector + DIR_DOWN) && (WhitelistedCoords.Contains(positionVector - DIR_UP) || isPathable(positionVector + DIR_DOWN))) {
                edges.Add(positionVector + DIR_DOWN);
            }

            // Right
            if (GeometryGrid.Contains(positionVector + DIR_RIGHT) && (WhitelistedCoords.Contains(positionVector - DIR_LEFT) || isPathable(positionVector + DIR_RIGHT))) {
                edges.Add(positionVector + DIR_RIGHT);
            }

            return edges;
        }
    }
}