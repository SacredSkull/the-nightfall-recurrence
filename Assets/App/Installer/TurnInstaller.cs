using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace Installer {
    public class TurnInstaller : MonoInstaller {
        public Settings SceneSettings;

        public override void InstallBindings() {
            Container.BindInstance(SceneSettings);
        }

        [Serializable]
        public class Settings {
            [Range(0.1f, 2f)]
            public float TimePerMove;
        }
    }
}
