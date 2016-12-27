using System.Collections.Generic;
using Controllers;
using Level;
using Level.Entity;
using thelab.mvc;

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
