using Controllers;
using Installer;
using Karma;
using Karma.Metadata;
using Presenters;
using Presenters.Layouts;
using UnityEngine;
using Zenject;

namespace App.Presenters {
    [Presenter(Path, LevelLayout.PrefabPath)]
    class LevelPresenter : MVCPresenter2D {
        private LevelController _levelController;
        public const string Path = "LevelView";

        [Inject]
        private void Constructor(LevelController levelController) {
            _levelController = levelController;
        }

        private void Start() {
            _levelController.SetupSelection(FindObjectOfType<SelectionPresenter>());
            _levelController.SetupGrid(FindObjectOfType<GridPresenter>());
        }
    }
}
