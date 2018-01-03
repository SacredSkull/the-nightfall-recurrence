using Controllers;
using Karma.Metadata;
using Presenters;
using Presenters.Layouts;
using UniRx;
using UnityEngine;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace App.Presenters {    
    [Presenter(Path, LevelLayout.PrefabPath)]
    public class LevelPresenter : MVCPresenter2D {
        public const string Path = "LevelView";
        private LevelController _levelController;
        private bool Ready = false;
        private ILogger _logger;

        [Inject]
        private void Constructor(LevelController levelController, ILogger logger) {
            _levelController = levelController;
            _logger = logger;
        }

        private void Start() {
            _levelController.SetupSelection(FindObjectOfType<SelectionPresenter>());
            _levelController.SetupGrid(FindObjectOfType<GridPresenter>());
            Ready = true;
            
            Observable.EveryUpdate().Subscribe(x => {
                _levelController._turnController.Tick();
            });

            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                .Subscribe(_ => _levelController.ParseMovement(KeyCode.W));
            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                .Subscribe(_ => _levelController.ParseMovement(KeyCode.S));
            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                .Subscribe(_ => _levelController.ParseMovement(KeyCode.A));
            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                .Subscribe(_ => _levelController.ParseMovement(KeyCode.D));
        }
    }
}
