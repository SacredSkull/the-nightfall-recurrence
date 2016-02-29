using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.Action.Move {

    public class GridGraph {
        private Dictionary<int, Dictionary<int, int>> grid;
        private HashSet<int> bad_path_IDs; 

        public GridGraph(Dictionary<int, Dictionary<int, int>> grid, HashSet<int> bad_paths) {
            this.grid = grid;
            this.bad_path_IDs = bad_paths;
        }

        public List<Vector2> Neighbours(Vector2 xy) {
            // Find neighbouring nodes that are not in the pathing black list
            // Check left, right, up down - diagonal movement is not possible.
            int x = (int)xy.x;
            int y = (int)xy.y;

            List<Vector2> edges = new List<Vector2>();

            // First, make sure the grid actually contains
            // this co-ordinate
            if (grid.ContainsKey(x) && grid[x].ContainsKey(y)) {
                // Left
                if (grid.ContainsKey(x - 1) && !bad_path_IDs.Contains(grid[x - 1][y]))
                {
                    edges.Add(new Vector2(x - 1, y));
                }

                // Right
                if (grid.ContainsKey(x + 1) && !bad_path_IDs.Contains(grid[x + 1][y]))
                {
                    edges.Add(new Vector2(x + 1, y));
                }

                // Up
                if (grid[x].ContainsKey(y - 1) && !bad_path_IDs.Contains(grid[x][y]))
                {
                    edges.Add(new Vector2(x, y - 1));
                }

                // Down
                if (grid[x].ContainsKey(y + 1) && !bad_path_IDs.Contains(grid[x][y]))
                {
                    edges.Add(new Vector2(x, y + 1));
                }
            }

            return edges;
        }
    }
    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public abstract class Governor {
        public virtual void move(GridGraph graph, Vector2 start_pos) {
            Queue<Vector2> frontier = new Queue<Vector2>();
            frontier.Enqueue(start_pos);

            HashSet<Vector2> visted_nodes = new HashSet<Vector2>();
            visted_nodes.Add(start_pos);

            while(frontier.Count != 0) {
                Vector2 current_pos = frontier.Dequeue();
                Utility.UnityLog(string.Format("[AI][PATHING] Visiting {0},{1}", current_pos.x, current_pos.y));
                foreach (Vector2 next_pos in graph.Neighbours(current_pos))
                {
                    if (visted_nodes.Contains(next_pos))
                        continue;
                    frontier.Enqueue(next_pos);
                    visted_nodes.Add(next_pos);
                }
            }
        }
    }
}