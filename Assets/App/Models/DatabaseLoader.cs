using UnityUtilities.Management;
using Zenject;

namespace Models {
	public abstract class DatabaseLoader : Loader {
        protected IDatabase db;
		protected ILogger logger;
        protected DatabaseLoader(IDatabase database) {
            db = database;
        }

		[Inject]
		public void SetupLogger(ILogger logger) {
			this.logger = logger;
		}

		public bool hasLoaded = false;
	}
}
