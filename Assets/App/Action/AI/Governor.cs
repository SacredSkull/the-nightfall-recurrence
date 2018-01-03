using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action.Ability;
using Gamelogic.Extensions.Algorithms;
using Installers;
using Level.Entity;
using Models;
using Presenters;
using UniRx;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Mapping.Pathing;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Action.AI {
    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public class Governor {
        protected static bool ExternalDataSet = false;
        protected static IGridGraph Graph;
        protected static ILogger logger;
        protected static IEnumerable<SoftwareTool> PlayerTools;
        protected static IEnumerable<SoftwareTool> SentryTools;
        public SoftwareTool thisTool { get; set; }
        [Inject]
        protected SoundPresenter _SoundPresenter;
        [Inject]
        protected AssetInstaller.SoundAssets SoundAssets;
        
        [Inject]
        public Governor(LevelModel lm, ILogger _logger, SoftwareTool tool) {
            if (!ExternalDataSet) {
                lm.LoadedEvent += () => {
                    Graph = lm.graph;
                    PlayerTools = lm.LevelHackTools;
                    SentryTools = lm.LevelSentries;
                };
                ExternalDataSet = true;
            }
            logger = _logger;
            thisTool = tool;
        }

        public PassingFilter[] StandardPassingFilters() {
            return new [] {new PassingFilter(thisTool.Tail.VectorList)};
        }

        public BlockingFilter[] StandardBlacklists() {
            return new[] {
                new BlockingFilter(PlayerTools.Concat(SentryTools).SelectMany(x => x.Tail.VectorList).ToArray()),
                new BlockingFilter(PlayerTools.Select(x => x.GetPosition()).ToArray()),
                new BlockingFilter(SentryTools.Except(new SoftwareTool[1] {thisTool}).Select(x => x.GetPosition())
                    .ToArray())
            };
        }

        public virtual IEnumerator TakeTurn(double timePerMove, double turnDelay) {
            KeyValuePair<SoftwareTool, IEnumerable<Vector2>> target = thisTool.Governor.SelectTarget();
            List<Vector2> path = target.Value.ToList();
				
            if(!path.IsEmpty())
                for (int i = 1; i <= thisTool.Movement; i++) {
                    //TODO: If we're at max size and in range of the target, why move?
                    //TODO: If we're NOT at max size and the target is in range, do something about it!
                    if (!(thisTool.AtMaxSize && Pathing.SnakeDistance(thisTool, target.Key) <= thisTool.LongestRangeAttack.Range)) {
                        path = thisTool.Governor.Move(path);
                        yield return Observable.Timer(TimeSpan.FromSeconds(timePerMove)).ToYieldInstruction();
                    } else {
                        logger.Log(
                            $"{thisTool.name} is in range of {target.Key.name} with {thisTool.LongestRangeAttack.Name} ({thisTool.LongestRangeAttack.Range})");
                        thisTool.Attack(thisTool.LongestRangeAttack, target.Key);
                        break;
                    }
                }
            yield return Observable.Timer(TimeSpan.FromSeconds(turnDelay)).ToYieldInstruction();
        }

        public virtual KeyValuePair<SoftwareTool, IEnumerable<Vector2>> SelectTarget() {
            // Find the closest tool using A star
            Dictionary<SoftwareTool, IList<Vector2>> toolPaths = new Dictionary<SoftwareTool, IList<Vector2>>();

            Pathing pathingAlgorithm = new AStar(Graph);
            pathingAlgorithm.SetBlockingFilters(StandardBlacklists());
            pathingAlgorithm.SetPassingFilters(StandardPassingFilters());
            
            foreach (SoftwareTool target in PlayerTools) {
                // TODO: When a tool is below its max health, it should *ahem* aim for the nearest tool, using a long(er) path. As the tool isn't at its best, it ideally wants to fill tiles AND then attack. To do this we need to find all paths that are no longer than our movement speed. The longest path at or below movement speed (maybe taking the difference of the health/maxhealth delta and the movement speed? e.g. 3/4 hp - 2 movement means we only need 1 extra tile - take a path at most +1 slower than the quickest) is perfect - we can grow and attack efficiently. If all paths are longer, we want the shortest path possible, because it we'll 100% use all our moves. The game doesn't really seem to take its own tiles into consideration, but a more refined AI could try checking for paths that avoid crossing over its own trail to grow even more. Probably unnecessary though.
                // BUG: Congrats! You've successfully set up pathing for single HP targets! Now do the same for every tile that it is apart of it (including the "head" too obviously!). You could cheat and find the nearest tool then only check its trail... but come on, have a sense of professionalism you cowboy!
                
                toolPaths.Add(target, pathingAlgorithm.AllPathsOf(thisTool.GetPosition(), target.GetPosition()));
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
                thisTool.Move(path[0]);
                _SoundPresenter.QueueAudio(SoundAssets.Movement);
//                Logger.UnityLog($"{tool.name} - Moved to {path[0]}");
                // Remove the path we just moved to
                path.RemoveAt(0);
                // TODO: the joining connections should probably be set too!
            }
            return path;
        }
    }

    public interface IGridVisualise {
        IEnumerator Visualise(Vector2 source, Vector2 dest);
    }
    
}