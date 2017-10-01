using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Level;
using ModestTree;
using Presenters;
using UnityEngine;
using Zenject;

namespace Installer {
    public class GameGridPresenterInstaller : MonoInstaller {
        public ContainerSettings containerSettings;
        public GridPieceSettings gridPieceSettings;
        public override void InstallBindings() {
            Container.BindFactory<TilePresenter, TilePresenter.Factory>().FromPrefab(gridPieceSettings.Tile);
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
