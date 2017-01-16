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
        private int turncounter = 0;

        public virtual IEnumerator TakeTurn(SoftwareTool thisTool, float timePerMove = 0.3f) {
            SoftwareTool[] tools = ServiceLocator.GetLevelEntityGrid().Values.Where(mi => !(mi.Value is Sentry) && mi.Value is SoftwareTool).Select(st => st.Value).Cast<SoftwareTool>().ToArray();

            // Find the closest tool using A star
            Dictionary<SoftwareTool, IList<Vector2>> toolPaths = new Dictionary<SoftwareTool, IList<Vector2>>();

            foreach (SoftwareTool target in tools) {
                // TODO: When a tool is below its max health, it should *ahem* aim for the nearest tool, using a long(er) path. As the tool isn't at its best, it ideally wants to fill tiles AND then attack. To do this we need to find all paths that are no longer than our movement speed. The longest path at or below movement speed (maybe taking the difference of the health/maxhealth delta and the movement speed? e.g. 3/4 hp - 2 movement means we only need 1 extra tile - take a path at most +1 slower than the quickest) is perfect - we can grow and attack efficiently. If all paths are longer, we want the shortest path possible, because it we'll 100% use all our moves. The game doesn't really seem to take its own tiles into consideration, but a more refined AI could try checking for paths that avoid crossing over its own trail to grow even more. Probably unnecessary though.
                // BUG: Congrats! You've successfully set up pathing for single HP targets! Now do the same for every tile that it is apart of it (including the "head" too obviously!). You could cheat and find the nearest tool then only check its trail... but come on, have a sense of professionalism you cowboy!
                toolPaths.Add(target, AllPathsOf(thisTool.GetPosition(), target.GetPosition(), thisTool.Tail.VectorList));
            }

            List<Vector2> shortestPath = new List<Vector2>();
            SoftwareTool targetTool = null;

            foreach (var path in toolPaths) {
                if(path.Value.Count < shortestPath.Count || shortestPath.Count == 0) {
                    shortestPath = new List<Vector2>(path.Value);
                    targetTool = path.Key;
                }
            }

            List<Attack.Attack> potentialAttacks = new List<Attack.Attack>();
            
            // TODO: The movement sound should be triggered in here when movement actually occurs. Currently the reference to the tool's gameobject is stored on the gridpiece, so no access from here atm.
            for(int i = 1; i <= thisTool.Movement; i++) {
                //TODO: If we're at max size and in range of the target, why move?
                //TODO: If we're NOT at max size and the target is in range, do something about it!
                if(!(thisTool.AtMaxSize && SnakeDistance(thisTool, targetTool) <= thisTool.LongestRangeAttack.Range))
                    shortestPath = Move(shortestPath, thisTool);
                else {
                    Logger.UnityLog($"{thisTool.name} is in range of {targetTool.name} with {thisTool.LongestRangeAttack.Name} ({thisTool.LongestRangeAttack.Range})");
                    break;
                }
                yield return new WaitForSeconds(timePerMove);

            }
			Logger.UnityLog($"At distance of {SnakeDistance(thisTool, targetTool)} to target!");
            potentialAttacks = thisTool.PotentialAttacks(SnakeDistance(thisTool, targetTool)).ToList();
            if (potentialAttacks.Count != 0) {
                Attack.Attack chosenAttack = potentialAttacks.OrderByDescending(x => x.Range).First();

                StringBuilder sb = new StringBuilder();
                foreach (var attack in potentialAttacks) {
                    sb.Append($"[{attack.Name}] Range: {attack.Range}");
                }
                Logger.UnityLog($"{sb} of {thisTool}");

                thisTool.Attack(chosenAttack, targetTool);
            }
        }

        private IList<Vector2> AllPathsOf(Vector2 startingPos, Vector2 destination, Vector2[] whitelist) {
            SortedList<int, IList<Vector2>> viablePaths = new SortedList<int, IList<Vector2>>(new DuplicateKeyComparer<int>());
            SortedList<int, IList<Vector2>> backupPaths = new SortedList<int, IList<Vector2>>(new DuplicateKeyComparer<int>());
            foreach(Vector2 neighbour in Graph.Neighbours(destination)) {
                KeyValuePair<bool, IList<Vector2>> result = AStarMovement(startingPos, neighbour, whitelist);
                if(result.Key)
                    viablePaths.Add(result.Value.Count, result.Value);
                else
                    backupPaths.Add(result.Value.Count, result.Value);
            }

            if(viablePaths.Count != 0) {
                //Logger.UnityLog($"Shortest ideal path is {viablePaths.First().Value.Count} long");
                return viablePaths.First().Value;
            }
            return backupPaths.Count != 0 ? backupPaths.First().Value : new List<Vector2>();
        }

        public virtual List<Vector2> Move(List<Vector2> path, SoftwareTool tool) {
            if(path != null && path.Count != 0) {
                Vector2 initialPos = tool.GetPosition();
                //entityLayer.Move(initialPos, path[0]);
                tool.Move(path[0]);
                
//                Logger.UnityLog($"{tool.name} - Moved to {path[0]}");
                // Remove the path we just moved to
                path.RemoveAt(0);
                // If our max size > 1, do stuff for the trail
                // BUG: ONLY new sections of a trail are being drawn/updated - need a repaint() function.

                // TODO: the joining connections should probably be set too!
                // If the current health/size == 1, then there are no tail points to update!                   
                
                // If the our current size == max size, don't add another point...
                // ...instead, we need to remove the last tail member.
                // Otherwise, add another tail point at our last position (stored in initialPos)

                //
            }

            return path;
        }

        public virtual IEnumerator DebugCalculatePath(Vector2 startPos, Vector2 destinationPos, bool debug) {
            return DebugAStarMovement(startPos, destinationPos, debug);
        }

        public static int SnakeDistance(IGridLocator first, IGridLocator second) {
			// BUG: Why is a null first or second being passed at all?
            return SnakeDistance(first.GetPosition(), second.GetPosition());
        }

        public static int SnakeDistance(Vector2 start, Vector2 finish) {
            return (int)(Math.Abs(start.x - finish.x) + Math.Abs(start.y - finish.y));
        }

        protected KeyValuePair<bool, IList<Vector2>> AStarMovement(Vector2 startPos, Vector2 destinationPos, Vector2[] whitelist) {
            SimplePriorityQueue<Vector2> frontier = new SimplePriorityQueue<Vector2>();
            frontier.Enqueue(startPos, 1);

            Dictionary<Vector2, Vector2> cameFromNode = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();
            cameFromNode[startPos] = startPos;
            costSoFar[startPos] = 0;

            bool pathable = false;
            Vector2 currentPos = new Vector2();

            while (frontier.Count > 0) {
                currentPos = frontier.Dequeue();
                //Logger.UnityLog($"[AI][PATHING] Visiting [{0},{1}]", currentPos.x, currentPos.y);

                // "Early exit" and check if the start and finish are identical
                if (currentPos.Equals(destinationPos)) {
                    break;
                }

                foreach (Vector2 nextPos in Graph.Neighbours(currentPos, whitelist)) {
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

            List<Vector2> path = new List<Vector2> {currentPos};

            // Add the final vector...

            // Unravel shortest path
            while (currentPos != startPos) {
                if (!cameFromNode.ContainsKey(currentPos) && currentPos != destinationPos)
                    break;

                path.Add(cameFromNode[currentPos]);
                currentPos = cameFromNode[currentPos];
            }
            
            return new KeyValuePair<bool, IList<Vector2>>(pathable, preparePath(path, startPos));
        }

        protected IList<Vector2> preparePath(List<Vector2> path, Vector2 startPos) {
            // We don't need/want the starting position.
            path.Remove(startPos);
            path.Reverse();

            //            StringBuilder stringPath = new StringBuilder();
            //            path.ForEach(x => stringPath.Append(x + ", "));
            //
            //            Logger.UnityLog($"[AI][PATHING] Path is {stringPath}");

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
                Logger.UnityLog($"[AI][PATHING] Visiting [{currentPos.x},{currentPos.y}]");

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

    /// <summary>
    /// Comparer for comparing two keys, handling equality as beeing greater
    /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable {
        public int Compare(TKey x, TKey y) {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }
    }
}