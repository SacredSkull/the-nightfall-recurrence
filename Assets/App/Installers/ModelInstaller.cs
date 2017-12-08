using Action.AI;
using Level;
using Level.Entity;
using Models;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Management;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Installers {
	public class ModelInstaller : MonoInstaller {
		public bool UseInternalDB;
		
		public override void InstallBindings() {
			// TODO: Setup a text logger implementation in UnityUtilities in case we're in a build and not the editor
			UnityEditorLogger logger = new UnityEditorLogger();
			Container.Bind<ILogger>().FromInstance(logger).AsSingle();
			Container.Bind<IDispatcher>().To<Utility.MainThreadDispatcher>().AsSingle();
			
			if (Application.isEditor && UseInternalDB) {
				// Use a fake database to speed up "live" tests
				Container.Bind<IDatabase>().To<FakeDatabase>().AsSingle();
				logger.Log("Not using the real dataset!", LogLevels.WARNING);
			} else {
				// Use the _real_ dataset
				Container.Bind<IDatabase>().To<XMLDatabase>().AsSingle();
			}
			
			Container.Bind<SpriteLoader>().AsCached();
			Container.Bind<LevelModel>().AsSingle();
			Container.Bind<EntityModel>().AsSingle();
		}
	}
}