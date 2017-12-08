using System.Collections.Generic;
using System.Text.RegularExpressions;
using Level;
using Level.Entity;
using UnityEngine;
using UnityUtilities.Management;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

namespace Models {
	public class SpriteLoader {
		public readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
		protected ILogger logger;

		[Inject]
		protected SpriteLoader(ILogger logger, IDispatcher mainThreadDispatcher) {
			mainThreadDispatcher.Post(x => {
				// I'm not sure whether to laugh or cry over these double assignments:
				loadedSprites["empty"] = MapItem.BlankTile.sprite = Resources.Load<Sprite>("Sprites/map_features/empty");
				loadedSprites["path"] = MapItem.MapPath.sprite = Resources.Load<Sprite>("Sprites/map_features/path");
			});
			
			this.logger = logger;
		}

	    public void Load(MapItem mi) {
			if (loadedSprites.ContainsKey(mi.string_id)) {
				mi.sprite = loadedSprites[mi.string_id];
				return;
			}

			// It's either an entity or a map feature.
		    Sprite sprite = Resources.Load<Sprite>($"Sprites/{mi.string_id}") ??
			                    Resources.Load<Sprite>($"Sprites/map_features/{mi.string_id}");
			if (sprite == null) {
				logger.Log($"[SPRITE] Could not find/load a sprite for {mi.string_id}", LogLevels.ERROR);
				return;
			}

			mi.sprite = sprite;
			loadedSprites.Add(mi.string_id, sprite);

            SoftwareTool tool = mi as SoftwareTool;
	        if(tool != null) {
                sprite = Resources.Load<Sprite>($"Sprites/{ Regex.Replace(tool.string_id, @"_\d", "_tail" ) }");
                if (sprite == null) {
	                logger.Log($"[SPRITE] Could not find/load a tail sprite for {mi.string_id}", LogLevels.ERROR);
                    return;
                }

	            tool.TailSprite = sprite;
	        }
	    }

	}
}
