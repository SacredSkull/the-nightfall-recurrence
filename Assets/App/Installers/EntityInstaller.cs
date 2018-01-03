using Action.AI;
using Level;
using Level.Entity;
using Zenject;

namespace Installers {
	public class EntityInstaller : MonoInstaller {
		public override void InstallBindings() {
			Container.Bind<Governor>().WithId("Standard").AsTransient();
			Container.Bind<Governor>().WithId("Ranged").To<RangedGovernor>().AsTransient();
			//Container.Bind<Governor>().WithId("Player").To<PlayerGovernor>().AsTransient();
			Container.Bind<RangedGovernor>().AsTransient();
			//Container.Bind<PlayerGovernor>().AsTransient();

			//Container.Bind<ObservedValue<GridGraph<MapItem>>>().FromInstance(Container.Resolve<LevelModel>().graph).AsSingle();
			
			//Container.Bind<ObservedValue<LayeredGrid<MapItem>>>().FromMethod(cont => Container.Resolve<LevelModel>().LayeredGrid).AsSingle();
			
			Container.BindFactory<MapItem, MapItem, MapItemFactory>();
			Container.BindFactory<HackTool, HackTool, HackToolFactory>();
			Container.BindFactory<Sentry, Sentry, SentryFactory>();
			Container.BindFactory<SoftwareTool, Trail, TrailFactory>();
			Container.BindFactory<SoftwareTool, GovernorNames, Governor, GovernorFactory>().FromFactory<CustomGovernorFactory>();
		}
	}
}