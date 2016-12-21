using Controllers;

namespace Models {
	public abstract class DatabaseLoader : Loader {
        protected IDatabase db;
        protected DatabaseLoader(IDatabase database) {
            db = database;
        }

        public DatabaseLoader() {
            
        }
	}
}
