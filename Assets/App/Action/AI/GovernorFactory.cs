using Level.Entity;
using Zenject;

namespace Action.AI {
	public class GovernorFactory : Factory<SoftwareTool, GovernorNames, Governor> {
	}
	
	public class CustomGovernorFactory : IFactory<SoftwareTool, GovernorNames, Governor> {
		private DiContainer _container;

		public CustomGovernorFactory(DiContainer cont) {
			_container = cont;
		}
		
		public Governor Create(SoftwareTool tool, GovernorNames gov) {
			if (gov == GovernorNames.RANGED)
				return _container.Instantiate<RangedGovernor>(new object[]{ tool });
			if (gov == GovernorNames.PLAYER)
				return _container.Instantiate<PlayerGovernor>(new object[]{ tool });
			return _container.Instantiate<Governor>(new object[]{ tool });
		}
	}
}