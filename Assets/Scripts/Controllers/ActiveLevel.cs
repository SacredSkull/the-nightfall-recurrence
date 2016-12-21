using System.Collections.Generic;
using System.Threading;
using Level;
using Models;
using thelab.mvc;
using UnityEngine;
using Utility.Collections.Grid;

namespace Controllers {
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

	    private bool levelLoaded;
	    private bool spritesLoaded;



        // TODO: This should really be on the GridPiece prefab (and the prefab should probably be called SoftwareTool and have the relevant object attached).

	    private void Awake() {
			ReadData();
	    }

	    protected override void Start () {
            // Using XML data, map sprites to appropriate entities
            // MUST take place AFTER loading, but BEFORE grid injection
//            LoadSprites();

            // Generate level from geometry/entity data
//            InjectGeometry();
//            InjectEntities();
			//ReadData();

        }

        // Update is called once per frame
        private void Update() {
//        if (Input.GetMouseButtonDown(0)) {
//            test?.Move(new Vector2(5, 7));
//            Debug.Log(string.Format("Attacking: {0}, Source: {1}", entityGrid.Get(1, 15).Value as Sentry, test));
//            //Debug.Log(test.Attacks);
//            test?.Attacks[0]?.Execute((SoftwareTool)entityGrid.Get(1, 15).Value, test);
//
//            //Debug.Log(string.Format("New position of {0} is {1}", entityGrid.Get(test.GetPosition()).Value.name, entityGrid.Get(test.GetPosition()).Value.GetPosition()));
//        }
	        if (levelLoaded) {
				throw new AbandonedMutexException();
	        }
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

	        level = new LevelModel(new XMLDatabase());

	        level.LevelLoaded += () => {
		        DataLoaded?.Invoke(level.graph, level.Sprites);
	        };
	        level.Load();
        }
    }
}
