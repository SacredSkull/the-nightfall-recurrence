using System.Collections.Generic;
using System.Threading;
using Level;
using Models;
using thelab.mvc;
using UnityEngine;
using Utility.Collections.Grid;

namespace Controllers {
    [ExecuteInEditMode]
    public class ActiveLevel : BaseApplication {
        public bool dumpLevelXML;
        public bool dumpProgramXML;
        public bool dumpEnemyXML;
        public bool dumpFeatureXML;
        public bool dumpLoadedMapItems;
        public bool dumpProgramAbilities;
        public bool dumpGeometry;
        public bool dumpEntities;
        public bool dumpTileSets;
        public bool dumpLevelEntities;
	    public LevelModel level;
	    public EntityModel entities;

	    public delegate void LevelLoadedHandler(GridGraph<MapItem> graph, Dictionary<string, Sprite> sprites);
	    public event LevelLoadedHandler DataLoaded;

	    private void Start() {
			ReadData();
	    }

        // Update is called once per frame
        private void Update() {

        }

        private void ReadData() {
	        EntityModel.dumpEnemyXML = dumpEnemyXML;
	        EntityModel.dumpFeatureXML = dumpFeatureXML;
	        EntityModel.dumpProgramAbilities = dumpProgramAbilities;
	        EntityModel.dumpProgramXML = dumpProgramXML;

	        LevelModel.dumpEntities = dumpEntities;
	        LevelModel.dumpLevelEntities = dumpLevelEntities;
	        LevelModel.dumpGeometry = dumpGeometry;
	        LevelModel.dumpLevelXML = dumpLevelXML;
	        LevelModel.dumpLoadedMapItems = dumpLoadedMapItems;
	        LevelModel.dumpTileSets = dumpTileSets;

            if(Application.isEditor && !Application.isPlaying) {
                level = new LevelModel(new FakeDatabase());
            } else {
                level = new LevelModel(new XMLDatabase());
            }

            level.LevelLoaded += () => {
		        DataReady(level.graph, level.Sprites);
	        };
	        level.Load();
        }

        protected void DataReady(GridGraph<MapItem> graph, Dictionary<string, Sprite> sprites) {
            DataLoaded?.Invoke(graph, sprites);
        }
    }
}
