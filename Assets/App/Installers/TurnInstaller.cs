using System;
using Controllers;
using UnityEngine;
using Zenject;

namespace Installers {
    public class TurnInstaller : MonoInstaller {
        public Settings SceneSettings;

        public override void InstallBindings() {
            Container.BindInterfacesAndSelfTo<TurnController>().AsSingle();
            Container.BindInstance(SceneSettings);
        }

        [Serializable]
        public class Settings {
            [Range(0.1f, 2f)]
            public float TimePerMove;
        }
    }
}
