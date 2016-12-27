using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using Priority_Queue;
using Level;
using Level.Entity;
using UnityEngine;
using Utility.Collections.Grid;
using Logger = Utility.Logger;

namespace Action.Movement {

    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public class Governor {
        private static readonly IGridGraph Graph = ServiceLocator.GetLevelGraph();
        private readonly IGridVisualise _visualiser = new GridVisualiser();
        private SoftwareTool slave;
        private int turncounter = 0;

        public Governor(SoftwareTool slave) {
            this.slave = slave;
        }

        public virtual void TakeTurn(Sentry thisTool) {
            MapItem[] tools = ServiceLocator.GetLevelEntityGrid().Values.Where(mi => !(mi.Value is Sentry) && mi.Value is SoftwareTool).Select(st => st.Value).ToArray();

            // Find the closest tool using A star
            List<IList<Vector2>> toolPaths = new List<IList<Vector2>>();

            foreach (var tool in tools) {
                toolPaths.Add(AStarMovement(thisTool.GetPosition(), tool.GetPosition()));
            }

            List<Vector2> shortestPath = new List<Vector2>();

            foreach (var path in toolPaths) {
                if (path.Count < shortestPath.Count || shortestPath.Count == 0)
                    shortestPath = new List<Vector2>(path);
            }

            
            // TODO: The movement sound should be triggered in here when movement actually occurs. Currently the reference to the tool's gameobject is stored on the gridpiece, so no access from here atm.
            // TODO: check if path intersects with other Sentries (probably in the A* function somehow...)

            Move(shortestPath, thisTool);
        }

        public virtual void Move(Vector2 destinationPos, Sentry tool) {
            if (destinationPos == tool.GetPosition())
                return;
            IList<Vector2> path = AStarMovement(tool.GetPosition(), destinationPos);

            IGridCollection<MapItem> entityLayer = ServiceLocator.GetLevelEntityGrid();
            entityLayer.Move(tool.GetPosition(), path[0]);
        }

        public virtual void Move(List<Vector2> path, Sentry tool) {
            IGridCollection<MapItem> entityLayer = ServiceLocator.GetLevelEntityGrid();
            if(path.Count != 0)
                entityLayer.Move(tool.GetPosition(), path[0]);
        }

        public virtual IEnumerator DebugCalculatePath(Vector2 startPos, Vector2 destinationPos, bool debug) {
            return DebugAStarMovement(startPos, destinationPos, debug);
        }

        protected static int SnakeDistance(Vector2 start, Vector2 finish) {
            return (int)(Math.Abs(start.x - finish.x) + Math.Abs(start.y - finish.y));
        }

        // TODO: Consider changing this to an all sources, all destinations algorithm, like Floyd-Warshall or Johnson's - probably a terrible idea though - this works.
        protected IList<Vector2> AStarMovement(Vector2 startPos, Vector2 destinationPos) {
            SimplePriorityQueue<Vector2> frontier = new SimplePriorityQueue<Vector2>();
            frontier.Enqueue(startPos, 1);

            Dictionary<Vector2, Vector2> cameFromNode = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
            cameFromNode[startPos] = startPos;
            costSoFar[startPos] = 0;

            bool pathable = false;

            while (frontier.Count > 0) {
                Vector2 currentPos = frontier.Dequeue();
                //Logger.UnityLog(string.Format("[AI][PATHING] Visiting [{0},{1}]", currentPos.x, currentPos.y));

                // "Early exit" and check if the start and finish are identical
                if (currentPos.Equals(destinationPos)) {
                    break;
                }

                foreach (Vector2 nextPos in Graph.Neighbours(currentPos)) {
                    pathable = true;
                    int newCost = costSoFar[currentPos] + Graph.Cost(nextPos);
                    if (!costSoFar.ContainsKey(nextPos) || newCost < costSoFar[nextPos]) {
                        costSoFar[nextPos] = newCost;
                        int priority = newCost + SnakeDistance(nextPos, destinationPos);

                        cameFromNode[nextPos] = currentPos;
                        if (frontier.Contains(nextPos))
                            frontier.UpdatePriority(nextPos, priority);
                        else
                            frontier.Enqueue(nextPos, priority);
                    }
                }
            }

            List<Vector2> path = new List<Vector2>();

            // This path is not traverseable (blocked, impassible)
            if (!pathable)
                return path;

            Vector2 current = destinationPos;

            // Add the final vector...
            path.Add(current);

            // Unravel shortest path
            while (current != startPos) {
                if (!cameFromNode.ContainsKey(current) && current != destinationPos)
                    break;

                path.Add(cameFromNode[current]);
                current = cameFromNode[current];
            }

            // We don't need/want the starting position.
            path.Remove(startPos);

            path.Reverse();

//            StringBuilder stringPath = new StringBuilder();
//            path.ForEach(x => stringPath.Append(x + ", "));

            //Logger.UnityLog(string.Format("[AI][PATHING] Path is {0}", stringPath));

            return path;
        }

        protected IEnumerator DebugAStarMovement(Vector2 startPos, Vector2 destinationPos, bool debug = false) {
            SimplePriorityQueue<Vector2> frontier = new SimplePriorityQueue<Vector2>();
            frontier.Enqueue(startPos, 1);

            Dictionary<Vector2, Vector2> cameFromNode = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
            cameFromNode[startPos] = startPos;
            costSoFar[startPos] = 0;

            while (frontier.Count > 0) {
                Vector2 currentPos = frontier.Dequeue();
                Logger.UnityLog(string.Format("[AI][PATHING] Visiting [{0},{1}]", currentPos.x, currentPos.y));

                // "Early exit"
                if (currentPos.Equals(destinationPos)) {
                    yield break;
                }

                foreach (Vector2 nextPos in Graph.Neighbours(currentPos)) {
                    int newCost = costSoFar[currentPos] + Graph.Cost(nextPos);
                    if (!costSoFar.ContainsKey(nextPos) || newCost < costSoFar[nextPos]) {
                        costSoFar[nextPos] = newCost;
                        int priority = newCost + SnakeDistance(nextPos, destinationPos);

                        cameFromNode[nextPos] = currentPos;
                        if (frontier.Contains(nextPos))
                            frontier.UpdatePriority(nextPos, priority);
                        else
                            frontier.Enqueue(nextPos, priority);
                    }
                }
                if (debug)
                    yield return _visualiser.Visualise(currentPos, frontier.First);
            }
        }
    }

    public interface IGridVisualise {
        IEnumerator Visualise(Vector2 source, Vector2 dest);
    }

    public class GridVisualiser : IGridVisualise {
        private static Sprite debugSprite = Resources.Load<Sprite>("Sprites/map_features/debug");
        public IEnumerator Visualise(Vector2 source, Vector2 dest) {
            GameObject go = ServiceLocator.GetLevelLayeredGrid().GetHighestElement(source).GameObject;
            GameObject nextGO = ServiceLocator.GetLevelLayeredGrid().GetHighestElement(dest).GameObject;

            Sprite currentSprite;
            Sprite nextSprite;

            SpriteRenderer currentRenderer = go.GetComponent<SpriteRenderer>();
            SpriteRenderer nextRenderer = nextGO.GetComponent<SpriteRenderer>();

            //DrawArrow.ForDebug(go.transform.position, nextGO.transform.position - go.transform.position);

            currentSprite = currentRenderer.sprite;
            nextSprite = nextRenderer.sprite;

            currentRenderer.sprite = debugSprite;
            nextRenderer.sprite = debugSprite;

            currentRenderer.color = Color.red;
            nextRenderer.color = Color.black;

            yield return null;

            currentRenderer.color = Color.yellow;
            nextRenderer.color = Color.blue;
        }
    }
}