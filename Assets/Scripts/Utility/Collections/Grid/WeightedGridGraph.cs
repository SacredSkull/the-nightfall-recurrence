using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.Collections.Grid {
    public interface INodeValidator {
        bool isPathable(int x, int y);
        bool isPathable(Vector2 coord);
    }

    public class GridGraph<T> : INodeValidator where T : IGridLocator {
        private readonly int EMPTY_ID = 0;
        private readonly int PATH_ID = 4;

        private LayeredGrid<T> LayeredGrid;
        private GridCollection<T> GeometryGrid;
        private GridCollection<T> EntityGrid;
        private HashSet<int> BlockedPathIDs;

        public GridGraph(LayeredGrid<T> layered, HashSet<int> BlockedPathIDs) {
            LayeredGrid = layered;
            GeometryGrid = layered.GetLayer("geometry");
            EntityGrid = layered.GetLayer("entity");
            this.BlockedPathIDs = BlockedPathIDs;

            if (GeometryGrid == null)
                throw new ArgumentNullException("geo", "The geometry grid is null");
            if (EntityGrid == null)
                throw new ArgumentNullException("ent", "The entity grid is null");
        }

        public bool isPathable(int x, int y) {
            GridPiece<T> geomePiece = GeometryGrid.Get(x, y);
            GridPiece<T> entPiece = EntityGrid.Get(x, y);

            if (geomePiece == null || entPiece == null)
                return false;

            return !BlockedPathIDs.Contains(geomePiece.ID);
        }

        public bool isPathable(Vector2 coord) {
            return isPathable((int) coord.x, (int) coord.y);
        }

        public List<Vector2> Neighbours(Vector2 xy) {
            // Find neighbouring nodes that are not in the pathing black list
            // Check left, right, up down - diagonal movement isn't possible,
            // but we still need to parse it!
            int x = (int) xy.x;
            int y = (int) xy.y;

            List<Vector2> edges = new List<Vector2>();

            // First, make sure the grid actually contains
            // this co-ordinate

            // Top-Left
            if (GeometryGrid.Contains(x - 1, y + 1) && isPathable(x - 1, y + 1)) {
                edges.Add(new Vector2(x - 1, y + 1));
            }

            // Left
            if (GeometryGrid.Contains(x - 1, y) && isPathable(x - 1, y)) {
                edges.Add(new Vector2(x - 1, y));
            }

            // Bottom-left
            if (GeometryGrid.Contains(x - 1, y - 1) && isPathable(x - 1, y - 1)) {
                edges.Add(new Vector2(x - 1, y - 1));
            }

            // Up
            if (GeometryGrid.Contains(x, y + 1) && isPathable(x, y + 1)) {
                edges.Add(new Vector2(x, y + 1));
            }

            // Down
            if (GeometryGrid.Contains(x, y - 1) && isPathable(x, y - 1)) {
                edges.Add(new Vector2(x, y - 1));
            }

            // Top-right
            if (GeometryGrid.Contains(x + 1, y + 1) && isPathable(x + 1, y + 1)) {
                edges.Add(new Vector2(x + 1, y + 1));
            }

            // Right
            if (GeometryGrid.Contains(x + 1, y) && isPathable(x + 1, y)) {
                edges.Add(new Vector2(x + 1, y));
            }

            // Bottom-right
            if (GeometryGrid.Contains(x + 1, y - 1) && isPathable(x + 1, y - 1)) {
                edges.Add(new Vector2(x + 1, y - 1));
            }

            return edges;
        }
    }
}