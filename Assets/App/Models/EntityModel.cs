using System.Collections.Generic;
using System.Linq;
using Action.Ability;
using Level;
using Level.Entity;
using UnityUtilities.Management;
using Zenject;

namespace Models {
	public class EntityModel : DatabaseLoader {
        public static bool dumpProgramXML;
        public static bool dumpEnemyXML;
        public static bool dumpFeatureXML;
        public static bool dumpProgramAbilities;

        public ICollection<HackTool> Tools;
        public ICollection<Sentry> SentryTools;
        public List<SoftwareTool> AllSoftware;
        public List<MapItem> AllFeatures;

		public MonolithicEvent EntitiesLoaded = new MonolithicEvent();
		protected IDispatcher MainThreadDispatcher;
		protected SpriteLoader sprites;

		[Inject]
		public EntityModel(IDatabase database, IDispatcher dispatcher, SpriteLoader spriteLoader) : base(database) {
			MainThreadDispatcher = dispatcher;
			sprites = spriteLoader;
		}

        public override void Load() {
	        EntitiesLoaded.Reset();
            Tools = db.loadHackTools();
            SentryTools = db.loadSentries();
            AllSoftware = SentryTools.Cast<SoftwareTool>().Concat(Tools.Cast<SoftwareTool>()).ToList();
            AllFeatures = db.loadMapItems();
            AllFeatures.Add(MapItem.BlankTile);
            AllFeatures.Add(MapItem.MapPath);
            AllFeatures.Add(SpawnPoint.Spawn);
			
            MainThreadDispatcher.Post(x => {
                foreach (SoftwareTool software in AllSoftware) {
                    sprites.Load(software);
                }

                foreach (MapItem feature in AllFeatures) {
                    sprites.Load(feature);
                }
                
	            logger.Log("[SPRITE] Holding " + sprites.loadedSprites.Count + " sprites.");
                Ready();
            });

            #region debug serialised SoftwareXML & Abilities

	        if(dumpProgramXML || dumpProgramAbilities)
				foreach (SoftwareTool tool in AllSoftware) {
					if (dumpProgramXML)
						logger.Log(tool.name);
					if (!dumpProgramAbilities)
						continue;

					foreach (Attack ability in tool.Attacks) {
						logger.Log("--->" + ability.Name);
					}
				}
            if (dumpFeatureXML)
                foreach (MapItem mapItem in AllFeatures) {
					logger.Log("[FEATURES] Found map feature " + mapItem.name);
                }

            if (dumpEnemyXML)
                foreach (SoftwareTool tool in SentryTools) {
					logger.Log(tool.name);
                }

            if (dumpProgramXML) {
                db.dumpHackTools();
                db.dumpSentries();
            }

            #endregion
        }

		protected override void Ready() {
			// We set the flag first, because everything will have already loaded, and listeners will already have attached themselves.
			hasLoaded = true;
			EntitiesLoaded.Fire();
		}
    }
}
