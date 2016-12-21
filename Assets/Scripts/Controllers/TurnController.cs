using System.Collections;
using System.Collections.Generic;
using Level;
using Level.Entity;
using thelab.mvc;
using UnityEngine;
using Utility.Collections.Grid;

namespace Controllers {
    public class TurnController : Controller<ActiveLevel> {
        private bool playerTurn;
        private bool turnWaiting;
        private int TurnCount = 1;
        private bool DataReady;

        // Use this for initialization
        private void Awake () {
	        app.DataLoaded += LoadingListener;
        }

	    private void LoadingListener(GridGraph<MapItem> graph, Dictionary<string, Sprite> sprites) {
		    DataReady = true;
	    }
	
        // Update is called once per frame
        private void Update () {
            if (!turnWaiting && DataReady) {
                StartCoroutine(playerTurn ? PlayerTurn() : SentriesTurn());
            }
        }

        private IEnumerator PlayerTurn() {
            turnWaiting = true;
            Debug.Log("Starting player turn...");
            yield return new WaitForSeconds(2);
            Debug.Log("...player turn has ended");
            playerTurn = false;
            turnWaiting = false;
	        TurnCount++;
        }

        private IEnumerator SentriesTurn() {
            turnWaiting = true;
            Debug.Log("Starting sentry turn...");
            foreach (Sentry sentry in app.level.LevelSentries) {
                // TODO: Movement noise is FAR TOO LOUD.
                for (int i = 0; i < sentry.Movement; i++) {
                    sentry.TakeTurn();
                    yield return new WaitForSeconds(0.3f);
                }
            }
            Debug.Log("...sentry turn has ended");
            playerTurn = true;
            turnWaiting = false;
	        TurnCount++;
        }
    }
}
