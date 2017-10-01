using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Level;
using Level.Entity;
using Models;
using UnityEngine;
using Utility;
using Logger = UnityUtilities.Logger;

namespace Models {
	public class SpriteLoader {
		public readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

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
			Sprite sprite = Resources.Load<Sprite>($"Sprites/{mi.string_id}") ??
							Resources.Load<Sprite>($"Sprites/map_features/{mi.string_id}");

			if (sprite == null) {
				Logger.UnityLog($"[SPRITE] Could not find/load a sprite for {mi.string_id}", LogLevels.ERROR);
				return;
			}


			// TODO: Remove or add a public debug toggle!
			//Logger.UnityLog($"[SPRITE] Added a sprite for '{mi.name}'@{sprite.name}");


			mi.sprite = sprite;
			loadedSprites.Add(mi.string_id, sprite);

            SoftwareTool tool = mi as SoftwareTool;
	        if(tool != null) {
                sprite = Resources.Load<Sprite>($"Sprites/{ Regex.Replace(tool.string_id, @"_\d", "_tail" ) }");
                if (sprite == null) {
                    Logger.UnityLog($"[SPRITE] Could not find/load a tail sprite for {mi.string_id}", LogLevels.ERROR);
                    return;
                }

	            tool.TailSprite = sprite;
	        }
	    }

	}
}
