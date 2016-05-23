using System.Collections;
using Level.Entity;
using UnityEngine;

namespace Level {
    public class LevelController : MonoBehaviour {
        public AudioClip MovementAudioClip;
        private bool playerTurn = false;
        private bool turnWaiting = false;
        private int TurnCount = 1;
        private GameController gc;
        private AudioSource source;
        private bool DataReady = true;

        // Use this for initialization
        private void Start () {
            gc = FindObjectOfType<GameController>();
            source = GetComponent<AudioSource>();
            gc.DataLoaded += LoadingListener;
        }

        private void LoadingListener() {
            DataReady = true;
        }
	
        // Update is called once per frame
        private void Update () {
            if (!turnWaiting) {
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
        }

        private IEnumerator SentriesTurn() {
            turnWaiting = true;
            Debug.Log("Starting sentry turn...");
            foreach (Sentry sentry in gc.LevelSentries) {
                // TODO: Movement noise is FAR TOO LOUD.
                for (int i = 0; i < sentry.Movement; i++) {
                    sentry.TakeTurn();
                    yield return new WaitForSeconds(0.3f);
                }
            }
            Debug.Log("...sentry turn has ended");
            playerTurn = true;
            turnWaiting = false;
        }
    }
}
