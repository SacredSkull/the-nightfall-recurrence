using Installers;
using Karma;
using Karma.Metadata;
using Level;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using Zenject;

namespace Presenters.Layouts {
    [Layout(PrefabPath)]
    class LevelLayout : MVCPresenter2D {
        public const string PrefabPath = "LevelLayout";
        public GameObject DebugGridOverlay;
        private Transform _overallContainer;

        [Inject]
        public void Constructor(GameGridPresenterInstaller.ContainerSettings containerSettings, ILayeredGrid<MapItem> layers) {
            _overallContainer = containerSettings.OverallContainer ?? transform;
            
            #if UNITY_EDITOR
            DebugGridOverlayPresenter debugPresenter = Instantiate(DebugGridOverlay, GameObject.Find("UI").transform).GetComponent<DebugGridOverlayPresenter>();
            debugPresenter.SetGrid(layers);
            #endif
        }

        public void Start() {
            transform.ResetTransformUnder(_overallContainer);
            transform.SetParent(_overallContainer);
        }
    }
}
