using System;
using Presenters;
using UnityEngine;
using Zenject;

namespace Installers {
    public class GameGridPresenterInstaller : MonoInstaller {
        public ContainerSettings containerSettings;
        public GridPieceSettings gridPieceSettings;
        public override void InstallBindings() {
            Container.BindFactory<TilePresenter, TilePresenter.Factory>().FromComponentInNewPrefab(gridPieceSettings.Tile);
            Container.BindInstance(containerSettings);
        }

        [Serializable]
        public class ContainerSettings {
            public Transform OverallContainer;
        }

        [Serializable]
        public class GridPieceSettings {
            public TilePresenter Tile;
        }
    }
}
