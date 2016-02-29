using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Scripts.Action.Move {

    public class GridGraph {
        public Dictionary<int, Dictionary<int, int>> grid;
        private HashSet<int> bad_path_IDs; 

        public GridGraph(Dictionary<int, Dictionary<int, int>> grid, HashSet<int> bad_paths) {
            this.grid = grid;
            this.bad_path_IDs = bad_paths;
        }

        public List<Vector2> Neighbours(Vector2 xy) {
            // Find neighbouring nodes that are not in the pathing black list
            // Check left, right, up down - diagonal movement isn't possible,
            // but we still need to parse it!
            int x = (int)xy.x;
            int y = (int)xy.y;

            List<Vector2> edges = new List<Vector2>();

            // First, make sure the grid actually contains
            // this co-ordinate

            // Top-Left
            if (edgeExists(x-1, y+1) && !bad_path_IDs.Contains(grid[x - 1][y + 1]))
            {
                edges.Add(new Vector2(x - 1, y + 1));
            }

            // Left
            if (edgeExists(x - 1, y) && !bad_path_IDs.Contains(grid[x - 1][y]))
            {
                edges.Add(new Vector2(x - 1, y));
            }

            // Bottom-left
            if (edgeExists(x - 1, y - 1) && !bad_path_IDs.Contains(grid[x - 1][y - 1]))
            {
                edges.Add(new Vector2(x - 1, y - 1));
            }

            // Up
            if (edgeExists(x, y + 1) && !bad_path_IDs.Contains(grid[x][y + 1]))
            {
                edges.Add(new Vector2(x, y + 1));
            }

            // Down
            if (edgeExists(x, y - 1) && !bad_path_IDs.Contains(grid[x][y - 1]))
            {
                edges.Add(new Vector2(x, y - 1));
            }

            // Top-right
            if (edgeExists(x + 1, y + 1) && !bad_path_IDs.Contains(grid[x + 1][y + 1]))
            {
                edges.Add(new Vector2(x + 1, y + 1));
            }

            // Right
            if (edgeExists(x + 1, y) && !bad_path_IDs.Contains(grid[x + 1][y]))
            {
                edges.Add(new Vector2(x + 1, y));
            }

            // Bottom-right
            if (edgeExists(x + 1, y - 1) && !bad_path_IDs.Contains(grid[x + 1][y - 1]))
            {
                edges.Add(new Vector2(x + 1, y - 1));
            }

            return edges;
        }

        private bool edgeExists(int x, int y) {
            return (grid.ContainsKey(x) && grid[x].ContainsKey(y));
        }
    }

    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public class Governor {
        private IGridVisualise visualiser = new GridVisualiser();

        public virtual void move(GridGraph graph, Vector2 start_pos) {
            Queue<Vector2> frontier = new Queue<Vector2>();
            frontier.Enqueue(start_pos);

            HashSet<Vector2> came_from_node = new HashSet<Vector2>();
            came_from_node.Add(start_pos);

            while(frontier.Count != 0) {
                Vector2 current_pos = frontier.Dequeue();
                Utility.UnityLog(string.Format("[AI][PATHING] Visiting {0},{1}", current_pos.x, current_pos.y));
                foreach (Vector2 next_pos in graph.Neighbours(current_pos)) {
                    if (came_from_node.Contains(next_pos))
                        continue;
                    frontier.Enqueue(next_pos);
                    came_from_node.Add(current_pos);
                }
                visualiser.Visualise(current_pos, frontier.Peek());
            }
        }
    }

    public interface IGridVisualise
    {
        void Visualise(Vector2 source, Vector2 dest);
    }

    public class GridVisualiser : IGridVisualise
    {
        public void Visualise(Vector2 source, Vector2 dest) {
            GameObject go = GameController.GetMapItemByCoords(source).Value.gameobject;
            Transform nextGO = GameController.GetMapItemByCoords(dest).Value.gameobject.transform;
            LineRenderer line = go.AddComponent<LineRenderer>();
            //line.useWorldSpace = true;
            line.SetVertexCount(2);
            line.SetWidth(10, 10);
            line.SetPosition(0, new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z - 2));
            line.SetPosition(1, new Vector3(nextGO.position.x, nextGO.position.y, nextGO.position.z - 2));
        }
    }
}