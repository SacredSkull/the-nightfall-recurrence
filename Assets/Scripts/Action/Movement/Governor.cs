using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility;
using Level;
using Level.Entity;
using Priority_Queue;
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

        public virtual void Move(Vector2 destinationPos, SoftwareTool tool) {
            if (destinationPos == tool.GetPosition())
                return;
            IList<Vector2> path = AStarMovement(tool.GetPosition(), destinationPos);

            IGridCollection<MapItem> entityLayer = ServiceLocator.GetLevelEntityGrid();
            entityLayer.Move(tool.GetPosition(), path[0]);
        }

        public virtual IEnumerator DebugCalculatePath(Vector2 startPos, Vector2 destinationPos, bool debug) {
            return DebugAStarMovement(startPos, destinationPos, debug);
        }

        protected static int SnakeDistance(Vector2 start, Vector2 finish) {
            return (int)(Math.Abs(start.x - finish.x) + Math.Abs(start.y - finish.y));
        }

        protected IList<Vector2> AStarMovement(Vector2 startPos, Vector2 destinationPos) {
            SimplePriorityQueue<Vector2> frontier = new SimplePriorityQueue<Vector2>();
            frontier.Enqueue(startPos, 1);

            Dictionary<Vector2, Vector2> cameFromNode = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
            cameFromNode[startPos] = startPos;
            costSoFar[startPos] = 0;

            while (frontier.Count > 0) {
                Vector2 currentPos = frontier.Dequeue();
                //Logger.UnityLog(string.Format("[AI][PATHING] Visiting [{0},{1}]", currentPos.x, currentPos.y));

                // "Early exit"
                if (currentPos.Equals(destinationPos)) {
                    break;
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
            }

            List<Vector2> path = new List<Vector2>();
            Vector2 current = destinationPos;

            // Add the final vector...
            path.Add(current);
            while (current != startPos) {
                if (cameFromNode.ContainsKey(current))
                    path.Add(cameFromNode[current]);
                current = cameFromNode[current];
            }

            // We don't need/want the starting position.
            path.Remove(startPos);

            path.Reverse();
            StringBuilder stringPath = new StringBuilder();
            path.ForEach(x => stringPath.Append(x + ", "));

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