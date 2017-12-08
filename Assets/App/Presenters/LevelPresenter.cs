using System.Threading;
using Controllers;
using Karma.Metadata;
using Presenters;
using Presenters.Layouts;
using UniRx;
using UniRx.Examples;
using UnityUtilities;
using UnityUtilities.Management;
using Utility;
using Zenject;

namespace App.Presenters {
    [Presenter(Path, LevelLayout.PrefabPath)]
    public class LevelPresenter : MVCPresenter2D {
        private LevelController _levelController;
        private TurnController _turnController;
        public const string Path = "LevelView";

        private bool Ready = false;
        private ILogger _logger;

        [Inject]
        private void Constructor(LevelController levelController, TurnController turnController, ILogger logger) {
            _levelController = levelController;
            _turnController = turnController;
            _logger = logger;
        }

        private void Start() {
            _levelController.SetupSelection(FindObjectOfType<SelectionPresenter>());
            _levelController.SetupGrid(FindObjectOfType<GridPresenter>());
            Ready = true;
            //_turnController.Start();

            var cpuTask = Observable.FromCoroutine(_turnController.CPUTurn).Subscribe();
            
//            Observable.EveryUpdate().Subscribe(x => {
//                _turnController.Tick();
//            });
        }
    }
}
