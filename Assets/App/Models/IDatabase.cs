using System.Collections.Generic;
using Level;
using Level.Entity;

namespace Models {
    public interface IDatabase {
        List<HackTool> loadHackTools();
        void dumpHackTools();

        List<Sentry> loadSentries();
        void dumpSentries();

        List<MapItem> loadMapItems();
        void dumpMapItems();

        Map loadLevel(string path);
        void dumpMap();
    }
}
