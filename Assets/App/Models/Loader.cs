using Utility;

namespace Models {
	public delegate void DataLoadedHandler();

	public abstract class Loader {
		public abstract void Load();
		protected abstract void Ready();
	}
}