using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action.Ability;
using Gamelogic.Extensions;
using Level;
using Level.Entity;
using Models;
using UniRx;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Mapping;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Action.AI {
    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public class Governor {
        protected static bool ExternalDataSet = false;
        protected static IGridCollection<MapItem> EntityGrid;
        protected static IGridGraph Graph;
        protected static ILogger logger;
        public SoftwareTool thisTool { get; set; }
        
        [Inject]
        public Governor(LevelModel lm, ILogger _logger, SoftwareTool tool) {
            if (!ExternalDataSet) {
                lm.LoadedEvent += () => {
                    Graph = lm.graph;
                    EntityGrid = lm.LayeredGrid.GetLayer(LayerNames.ENTITY_LAYER);
                };
                ExternalDataSet = true;
            }
            logger = _logger;
            thisTool = tool;
        }

        public virtual KeyValuePair<SoftwareTool, IEnumerable<Vector2>> SelectTarget() {
            SoftwareTool[] tools = EntityGrid.Values.Where(mi => !(mi.Value is Sentry) && mi.Value is SoftwareTool).Select(st => st.Value).Cast<SoftwareTool>().ToArray();

            // Find the closest tool using A star
            Dictionary<SoftwareTool, IList<Vector2>> toolPaths = new Dictionary<SoftwareTool, IList<Vector2>>();

            foreach (SoftwareTool target in tools) {
                // TODO: When a tool is below its max health, it should *ahem* aim for the nearest tool, using a long(er) path. As the tool isn't at its best, it ideally wants to fill tiles AND then attack. To do this we need to find all paths that are no longer than our movement speed. The longest path at or below movement speed (maybe taking the difference of the health/maxhealth delta and the movement speed? e.g. 3/4 hp - 2 movement means we only need 1 extra tile - take a path at most +1 slower than the quickest) is perfect - we can grow and attack efficiently. If all paths are longer, we want the shortest path possible, because it we'll 100% use all our moves. The game doesn't really seem to take its own tiles into consideration, but a more refined AI could try checking for paths that avoid crossing over its own trail to grow even more. Probably unnecessary though.
                // BUG: Congrats! You've successfully set up pathing for single HP targets! Now do the same for every tile that it is apart of it (including the "head" too obviously!). You could cheat and find the nearest tool then only check its trail... but come on, have a sense of professionalism you cowboy!
                toolPaths.Add(target, Pathing.AllPathsOf(thisTool.GetPosition(), target.GetPosition(), thisTool.Tail.VectorList, Graph));
            }

            List<Vector2> shortestPath = new List<Vector2>();
            SoftwareTool targetTool = null;

            foreach (var path in toolPaths) {
                if(path.Value.Count < shortestPath.Count || shortestPath.Count == 0) {
                    shortestPath = new List<Vector2>(path.Value);
                    targetTool = path.Key;
                }
            }
            
            return new KeyValuePair<SoftwareTool, IEnumerable<Vector2>>(targetTool, shortestPath);
        }

        public virtual void PickAttack(SoftwareTool targetTool) {
            List<Attack> potentialAttacks = new List<Attack>();
            
            // TODO: The movement sound should be triggered in here when movement actually occurs. Currently the reference to the tool's gameobject is stored on the gridpiece, so no access from here atm.
            
            logger.Log($"At distance of {(thisTool, targetTool)} to target!");
            potentialAttacks = thisTool.PotentialAttacks(Pathing.SnakeDistance(thisTool, targetTool)).ToList();
            if (potentialAttacks.Count != 0) {
                Attack chosenAttack = potentialAttacks.OrderByDescending(x => x.Range).First();

                StringBuilder sb = new StringBuilder();
                foreach (var attack in potentialAttacks) {
                    sb.Append($"[{attack.Name}] Range: {attack.Range}");
                }
                logger.Log($"{sb} of {thisTool}");

                thisTool.Attack(chosenAttack, targetTool);
            }
        }

        public virtual List<Vector2> Move(List<Vector2> path) {
            if(path != null && path.Count != 0) {
                Vector2 initialPos = thisTool.GetPosition();
                //entityLayer.Move(initialPos, path[0]);
                thisTool.Move(path[0]);
                
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



        
    }

    public interface IGridVisualise {
        IEnumerator Visualise(Vector2 source, Vector2 dest);
    }

//    public class GridVisualiser : IGridVisualise {
//        private static Sprite debugSprite = Resources.Load<Sprite>("Sprites/map_features/debug");
//        public IEnumerator Visualise(Vector2 source, Vector2 dest) {
//            GameObject go = ServiceLocator.GetLevelLayeredGrid().GetHighestElement(source).Value.GameObject;
//            GameObject nextGO = ServiceLocator.GetLevelLayeredGrid().GetHighestElement(dest).Value.GameObject;
//
//            Sprite currentSprite;
//            Sprite nextSprite;
//
//            SpriteRenderer currentRenderer = go.GetComponent<SpriteRenderer>();
//            SpriteRenderer nextRenderer = nextGO.GetComponent<SpriteRenderer>();
//
//            //DrawArrow.ForDebug(go.transform.position, nextGO.transform.position - go.transform.position);
//
//            currentSprite = currentRenderer.sprite;
//            nextSprite = nextRenderer.sprite;
//
//            currentRenderer.sprite = debugSprite;
//            nextRenderer.sprite = debugSprite;
//
//            currentRenderer.color = Color.red;
//            nextRenderer.color = Color.black;
//
//            yield return null;
//
//            currentRenderer.color = Color.yellow;
//            nextRenderer.color = Color.blue;
//        }
//    }

    
}