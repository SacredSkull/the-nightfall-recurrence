using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using Installers;
using Level.Entity;
using Models;
using UniRx;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Management;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Controllers {	
	public class TurnController : IDisposable {
		public enum PlayerTurnState {
			Unselected,
			Selected,
			Targeting,
			End
		}
		
		public enum TurnStates {
			PlayerTurn,
			CPUTurn,
			GameOver
		}
		
		private int TurnCount = 1;
		private bool DataReady;
		protected TurnInstaller.Settings turnSettings;


		protected ILogger Logger { get; set; }
		protected OverlayController _overlayController;
		protected List<Sentry> LevelSentries;
		protected List<HackTool> LevelHackTools;
		protected StateMachine<TurnStates> TurnStateMachine;
		protected StateMachine<PlayerTurnState> PlayerStateMachine;
		protected IGridGraph graph;
		public List<Sentry> FinishedSentries;
		public List<HackTool> FinishedHackTools;

		public TurnStates TurnState => TurnStateMachine.CurrentState;
		public PlayerTurnState PlayerState => PlayerStateMachine.CurrentState;
		
		[Inject]
		public void Init(ILogger logger, LevelModel lm, OverlayController overlay, TurnInstaller.Settings settings) {
			turnSettings = settings;
			
			FinishedHackTools = new List<HackTool>();
			FinishedSentries = new List<Sentry>();
			
			PlayerStateMachine = new StateMachine<PlayerTurnState>();
			PlayerStateMachine.AddState(PlayerTurnState.End, TestPlayerState);
			PlayerStateMachine.AddState(PlayerTurnState.Selected, TestPlayerState);
			PlayerStateMachine.AddState(PlayerTurnState.Targeting, TestPlayerState);
			PlayerStateMachine.AddState(PlayerTurnState.Unselected, TestPlayerState);
			
			TurnStateMachine = new StateMachine<TurnStates>();
			TurnStateMachine.AddState(TurnStates.PlayerTurn, PlayerTurn, null, TestTurnState);
			TurnStateMachine.AddState(TurnStates.CPUTurn, CPUTurn, null, TestTurnState);
			TurnStateMachine.AddState(TurnStates.GameOver, GameOver);
			
			lm.LoadedEvent.Subscribe(() => {
				LevelSentries = lm.LevelSentries;
				LevelHackTools = lm.LevelHackTools;
				TurnStateMachine.CurrentState = TurnStates.PlayerTurn;
				graph = lm.graph;
			});

			_overlayController = overlay;
			Logger = logger;
			
		}

		private void TestPlayerState() {
			Logger.Log($"Changed state to @{PlayerStateMachine.CurrentState}");
		}
		
		private void TestTurnState() {
			Logger.Log($"Changed state to @{TurnStateMachine.CurrentState}");
		}

		public void Tick() {
			PlayerStateMachine.Update();
			if(TurnStateMachine.CurrentState != TurnStates.GameOver)
				TurnStateMachine.Update();
			if (LevelHackTools.Count == 0 || LevelSentries.Count == 0)
				TurnStateMachine.CurrentState = TurnStates.GameOver;
		}

		public void PlayerSelectedTool(HackTool tool) {
			PlayerStateMachine.CurrentState = PlayerTurnState.Selected;
			_overlayController.ClearAllOverlays();
		}

		public void PlayerDeselectedTool() {
			if(PlayerStateMachine.CurrentState != PlayerTurnState.Targeting)
				PlayerStateMachine.CurrentState = PlayerTurnState.Unselected;
		}

		public void GameOver() {
			if(LevelHackTools.Count == 0)
				Logger.Log("YOU LOSE! YOU GET NOTHING!", LogLevels.INFO);
			else
				Logger.Log("Incredible! You won without the ability to defend yourself!", LogLevels.INFO);
		}

		public void PlayerTurn() {
			Logger.Log("Starting player turn...", LogLevels.INFO);
			PlayerStateMachine.CurrentState = PlayerTurnState.Unselected;
			foreach(HackTool tool in LevelHackTools)
				tool.ResetMovement();
		}

		public void EndPlayerTurn() {
			PlayerStateMachine.CurrentState = PlayerTurnState.End;
			Logger.Log("...ending player turn", LogLevels.INFO);
			TurnStateMachine.CurrentState = TurnStates.CPUTurn;
		}

		public void CPUTurn() {
			Observable.FromCoroutine(CPUs)
				.ObserveOnMainThread()
				.Subscribe(x => TurnStateMachine.CurrentState = TurnStates.PlayerTurn);
		}
		
		public IEnumerator CPUs() {
			Logger.Log("Starting sentry turn...", LogLevels.INFO);
			FinishedSentries = new List<Sentry>();
			foreach (Sentry sentry in LevelSentries) {
				yield return sentry.Governor.TakeTurn(turnSettings.TimePerMove, turnSettings.BreakBetweenCPUs);
				FinishedSentries.Add(sentry);
			}
			Logger.Log("...sentry turn has ended", LogLevels.INFO);
			TurnCount++;
		}

		public void Dispose() {
			
		}
	}
}
