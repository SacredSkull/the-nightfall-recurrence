using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gamelogic.Extensions;
using Installers;
using Level.Entity;
using Models;
using UniRx;
using UnityEngine;
using UnityUtilities.Mapping;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Controllers {
	public enum TurnStates {
		PlayerTurn,
		CPUTurn,
	}

	public class TurnController : IInitializable, IDisposable {
		private int TurnCount = 1;
		private bool DataReady;

		protected ILogger Logger { get; set; }
		protected List<Sentry> LevelSentries;
		protected List<HackTool> LevelHackTools;
		protected float timePerMove;

		[Inject]
		public TurnController(ILogger logger, LevelModel lm, EntityModel em, TurnInstaller.Settings settings) {
			timePerMove = settings.TimePerMove;
			lm.LoadedEvent.Subscribe(() => {
				LevelSentries = lm.LevelSentries;
				LevelHackTools = lm.LevelHackTools;
			});
			Logger = logger;
		}

		public void Initialize() {
		}

		public void Start() {
		}
	
		public void Tick () {
		}

		public IEnumerator CommonTurn() {
			// TODO: Is this needed?
			yield return null;
		}

		public IEnumerator PlayerTurn() {
			Logger.Log("Starting player turn...");
			yield return CommonTurn();
			foreach (var tool in LevelHackTools) {
				
			}
			yield return Observable.Timer(TimeSpan.FromSeconds(1)).ToYieldInstruction();
			Logger.Log("...player turn has ended");				
		}
		
		public IEnumerator CPUTurn() {
			Logger.Log("Starting sentry turn...");
			yield return CommonTurn();
			foreach (Sentry sentry in LevelSentries) {
				KeyValuePair<SoftwareTool, IEnumerable<Vector2>> target = sentry.Governor.SelectTarget();
				List<Vector2> path = target.Value.ToList();

				for (int i = 1; i <= sentry.Movement; i++) {
					//TODO: If we're at max size and in range of the target, why move?
					//TODO: If we're NOT at max size and the target is in range, do something about it!
					if (!(sentry.AtMaxSize && Pathing.SnakeDistance(sentry, target.Key) <= sentry.LongestRangeAttack.Range)) {
						path = sentry.Governor.Move(path);
						yield return Observable.Timer(TimeSpan.FromMilliseconds(500)).ToYieldInstruction();
					} else {
						Logger.Log(
							$"{sentry.name} is in range of {target.Key.name} with {sentry.LongestRangeAttack.Name} ({sentry.LongestRangeAttack.Range})");
						break;
					}
				}

			}
			Logger.Log("...sentry turn has ended");
		}

		public void Dispose() {
			
		}
	}
}
