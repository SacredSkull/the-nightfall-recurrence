using UnityUtilities.Management;

namespace Models {
	public abstract class Loader {
		public MonolithicEvent LoadedEvent = new MonolithicEvent();
		public abstract void Load();
		protected abstract void Ready();
	}
}