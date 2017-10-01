using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Installer;
using Karma;
using Karma.Metadata;
using Presenters;
using UnityEngine;
using Zenject;

namespace Presenters.Layouts {
    [Layout(PrefabPath)]
    class LevelLayout : MVCPresenter2D {
        public const string PrefabPath = "LevelLayout";
        private Transform _overallContainer;

        [Inject]
        public void Constructor(GameGridPresenterInstaller.ContainerSettings containerSettings) {
            _overallContainer = containerSettings.OverallContainer ?? transform;
        }

        public void Start() {
            transform.ResetTransformUnder(_overallContainer);
            transform.SetParent(_overallContainer);
        }
    }
}
