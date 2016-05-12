using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Utility;
using Utility.Collections;
using Utility.Collections.Grid;
using Logger = Utility.Logger;

namespace Action.Move {

    // Governor sets the movement algorithm,
    // AND the targeting details.
    // For example, the ranged enemies will want to get in range
    // but only just!
    public class Governor {        
        private readonly IGridVisualise _visualiser = new GridVisualiser();

        public virtual IEnumerator Move(GridGraph<MapItem> graph, Vector2 startPos, Vector2 destinationPos, bool debug = false) {
            Queue<Vector2> frontier = new Queue<Vector2>();
            frontier.Enqueue(startPos);

            HashSet<Vector2> cameFromNode = new HashSet<Vector2>();
            cameFromNode.Add(startPos);

            while(frontier.Count != 0) {
                Vector2 currentPos = frontier.Dequeue();
                Logger.UnityLog(string.Format("[AI][PATHING] Visiting [{0},{1}]", currentPos.x, currentPos.y));

                if (currentPos == destinationPos) {
                    yield break;
                }

                foreach (Vector2 nextPos in graph.Neighbours(currentPos)) {
                    if (cameFromNode.Contains(nextPos))
                        continue;
                    frontier.Enqueue(nextPos);
                    cameFromNode.Add(currentPos);
                }
                if (debug)
                    yield return _visualiser.Visualise(currentPos, frontier.Peek());
            }
        }
    }

    public interface IGridVisualise {
        IEnumerator Visualise(Vector2 source, Vector2 dest);
    }

    public class GridVisualiser : IGridVisualise {
        private static Sprite debugSprite = Resources.Load<Sprite>("Sprites/map_features/debug");
        public IEnumerator Visualise(Vector2 source, Vector2 dest) {
            GameObject go = GameController.LayeredGrid.GetHighestElement(source).Value.gameobject;
            GameObject nextGO = GameController.LayeredGrid.GetHighestElement(dest).Value.gameobject;

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


//            currentRenderer.sprite = currentSprite;
//            nextRenderer.sprite = nextSprite;

            //line.useWorldSpace = true;
//            line.SetVertexCount(2);
//            line.SetWidth(0.015f, 0.015f);
//            line.SetPosition(0, new Vector3(go.transform.position.x, go.transform.position.y, -go.transform.position.z));
//            line.SetPosition(1, new Vector3(nextGO.position.x, nextGO.position.y, -nextGO.position.z));
        }
    }
}