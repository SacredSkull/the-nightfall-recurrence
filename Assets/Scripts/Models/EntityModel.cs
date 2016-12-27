using System;
using System.Collections.Generic;
using System.Linq;
using Action.Attack;
using Level;
using Level.Entity;
using UnityEngine.Networking.NetworkSystem;
using Utility;

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

		public delegate void EntitiesLoadedHandler();

		public event EntitiesLoadedHandler EntitiesLoaded;

        public EntityModel(IDatabase database) : base(database) {}

        public override void Load() {
            Tools = db.loadHackTools();
            SentryTools = db.loadSentries();
            AllSoftware = SentryTools.Cast<SoftwareTool>().Concat(Tools.Cast<SoftwareTool>()).ToList();
            AllFeatures = db.loadMapItems();
            AllFeatures.Add(MapItem.BlankTile);
            AllFeatures.Add(MapItem.MapPath);
            AllFeatures.Add(SpawnPoint.Spawn);

	        SpriteLoader sprites = new SpriteLoader();
	        foreach (SoftwareTool software in AllSoftware) {
		        sprites.Load(software);
	        }

	        foreach (MapItem feature in AllFeatures) {
		        sprites.Load(feature);
	        }

	        Ready();

            #region debug serialised SoftwareXML & Abilities

	        if(dumpProgramXML || dumpProgramAbilities)
				foreach (SoftwareTool tool in AllSoftware) {
					if (dumpProgramXML)
						Logger.UnityLog(tool.name);
					if (!dumpProgramAbilities)
						continue;

					foreach (Attack ability in tool.Attacks) {
						Logger.UnityLog("---->" + ability.Name);
					}
				}
            if (dumpFeatureXML)
                foreach (MapItem mapItem in AllFeatures) {
                    Logger.UnityLog("[FEATURES] Found map feature " + mapItem.name);
                }

            if (dumpEnemyXML)
                foreach (SoftwareTool tool in SentryTools) {
                    Logger.UnityLog(tool.name);
                }

            if (dumpProgramXML) {
                db.dumpHackTools();
                db.dumpSentries();
            }

            #endregion
        }

		protected override void Ready() {
			//throw new NotImplementedException();
			EntitiesLoaded?.Invoke();
		}
    }
}
