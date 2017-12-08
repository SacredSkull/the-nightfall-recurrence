using JetBrains.Annotations;
using Zenject;

namespace Level.Entity {
	public class MapItemFactory : Factory<MapItem, MapItem> {
		public MapItem CreateBlankTile() {
			return Create(MapItem.BlankTile);
		}

		public MapItem CreateMapPath() {
			return Create(MapItem.MapPath);
		}
	}

	[UsedImplicitly]
	public class SentryFactory : Factory<Sentry, Sentry> {
		
	}

	[UsedImplicitly]
	public class HackToolFactory : Factory<HackTool, HackTool> {
		
	}
}