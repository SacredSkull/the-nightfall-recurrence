using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gamelogic.Extensions;
using Installer;
using Karma;
using Karma.Metadata;
using Level.Entity;
using Models;
using Utility;
using Zenject;

namespace Controllers {
    public enum TurnStates {
        PlayerTurn,
        CPUTurn
    }

    [Controller]
    public class TurnController : IController, ITickable, IInitializable {
        private int TurnCount = 1;
        private bool DataReady;
        public StateMachine<TurnStates> TurnSM;

        protected ILogger Logger { get; set; }
        protected List<Sentry> LevelSentries;
        protected float timePerMove;


        [Inject]
        public TurnController(ILogger logger, EntityModel em, TurnInstaller.Settings settings) {
            timePerMove = settings.TimePerMove;
            LevelSentries = em.SentryTools.ToList();
            Logger = logger;
        }

        public void Initialize() {
            TurnSM = new StateMachine<TurnStates>();
            TurnSM.AddState(TurnStates.CPUTurn, SentriesTurn);
            TurnSM.AddState(TurnStates.PlayerTurn, PlayerTurn);
        }

        //[ContextMenu("Step turn")]
        private void StepTurn() {
            TurnSM.CurrentState = TurnStates.CPUTurn;
        }
	
        // Update is called once per frame
        public void Tick () {
            TurnSM.Update();
        }

        private void PlayerTurn() {
            Logger.Log("Starting player turn...");
            Thread.Sleep(TimeSpan.FromSeconds(timePerMove));
            Logger.Log("...player turn has ended");
	        TurnCount++;

            TurnSM.CurrentState = TurnStates.CPUTurn;
        }

        private void SentriesTurn() {
            Logger.Log("Starting sentry turn...");
            foreach (Sentry sentry in LevelSentries) {
                sentry.TakeTurn(timePerMove);
            }
            Logger.Log("...sentry turn has ended");
	        TurnCount++;

            TurnSM.CurrentState = TurnStates.PlayerTurn;
        }

        public void OnDestroy() {
            
        }
    }
}
