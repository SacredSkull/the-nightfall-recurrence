using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Utility.Collections;
using Logger = Utility.Logger;

namespace Scripts.Action.Move {
    public class GridGraph<T> {
        public GridCollection<T> grid;
        private HashSet<int> bad_path_IDs; 

        public GridGraph(GridCollection<T> grid, HashSet<int> bad_paths) {
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
            if (grid.Contains(x-1, y+1) && !bad_path_IDs.Contains(grid.Get(x - 1,y + 1).id))
            {
                edges.Add(new Vector2(x - 1, y + 1));
            }

            // Left
            if (grid.Contains(x - 1, y) && !bad_path_IDs.Contains(grid.Get(x - 1,y).id))
            {
                edges.Add(new Vector2(x - 1, y));
            }

            // Bottom-left
            if (grid.Contains(x - 1, y - 1) && !bad_path_IDs.Contains(grid.Get(x - 1,y - 1).id))
            {
                edges.Add(new Vector2(x - 1, y - 1));
            }

            // Up
            if (grid.Contains(x, y + 1) && !bad_path_IDs.Contains(grid.Get(x, y + 1).id))
            {
                edges.Add(new Vector2(x, y + 1));
            }

            // Down
            if (grid.Contains(x, y - 1) && !bad_path_IDs.Contains(grid.Get(x, y - 1).id))
            {
                edges.Add(new Vector2(x, y - 1));
            }

            // Top-right
            if (grid.Contains(x + 1, y + 1) && !bad_path_IDs.Contains(grid.Get(x + 1, y + 1).id))
            {
                edges.Add(new Vector2(x + 1, y + 1));
            }

            // Right
            if (grid.Contains(x + 1, y) && !bad_path_IDs.Contains(grid.Get(x + 1, y).id))
            {
                edges.Add(new Vector2(x + 1, y));
            }

            // Bottom-right
            if (grid.Contains(x + 1, y - 1) && !bad_path_IDs.Contains(grid.Get(x + 1, y - 1).id))
            {
                edges.Add(new Vector2(x + 1, y - 1));
            }

            return edges;
        }
    }

    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public class Governor
    {
        public static GridCollection<MapItem> grid; 
        private IGridVisualise visualiser = new GridVisualiser();

        public virtual void move(GridGraph<MapItem> graph, Vector2 start_pos) {
            Queue<Vector2> frontier = new Queue<Vector2>();
            frontier.Enqueue(start_pos);

            HashSet<Vector2> came_from_node = new HashSet<Vector2>();
            came_from_node.Add(start_pos);

            while(frontier.Count != 0) {
                Vector2 current_pos = frontier.Dequeue();
                Logger.UnityLog(string.Format("[AI][PATHING] Visiting {0},{1}", current_pos.x, current_pos.y));
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