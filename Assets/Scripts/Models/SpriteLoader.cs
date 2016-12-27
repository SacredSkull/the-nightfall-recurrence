using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Level;
using Models;
using UnityEngine;
using Logger = Utility.Logger;

namespace Models {
	public class SpriteLoader {
		public event DataLoadedHandler SpritesLoaded;

		public static readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

		public SpriteLoader() {
			// I'm not sure whether to laugh or cry over these double assignments:
			loadedSprites["empty"] = MapItem.BlankTile.sprite = Resources.Load<Sprite>("Sprites/map_features/empty");
			loadedSprites["path"] = MapItem.MapPath.sprite = Resources.Load<Sprite>("Sprites/map_features/path");
		}

	    public void Load(MapItem mi) {
			if (loadedSprites.ContainsKey(mi.string_id)) {
				mi.sprite = loadedSprites[mi.string_id];
				return;
			}

			// It's either an entity or a map feature.
			Sprite sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", mi.string_id)) ??
							Resources.Load<Sprite>(string.Format("Sprites/map_features/{0}", mi.string_id));

			if (sprite == null) {
				Logger.UnityLog(string.Format("[SPRITE] Could not find/load a sprite for {0}", mi.string_id), Logger.Level.ERROR);
				return;
			}

			// TODO: Remove or add a public debug toggle!
			Logger.UnityLog(string.Format("[SPRITE] Added a sprite for '{0}'@{1}", mi.name, sprite.name));


			mi.sprite = sprite;
			loadedSprites.Add(mi.string_id, sprite);

		    Logger.UnityLog("[SPRITE] Holding " + loadedSprites.Count + " sprites.");
		    SpritesLoaded?.Invoke();

		    return;
	    }

	}
}
