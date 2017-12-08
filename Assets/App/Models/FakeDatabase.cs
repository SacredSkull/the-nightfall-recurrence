using System;
using System.Collections.Generic;
using Action.Ability;
using Level;
using Level.Entity;

namespace Models {
	public class FakeDatabase : XMLDatabase {
		public override List<HackTool> loadHackTools() {
			if (hackTools != null)
				return hackTools;

			return hackTools = new List<HackTool> {
				new HackTool {
					Attacks = new List<Attack> {
						new AttackBasic {
							damage = 2,
							Name = "Slice",
							Range = 1
						}
					},
					Level = 1,
					Cost = 500,
					MaxHealth = 2,
					Movement = 3,
					description = "Comes with the hacking starter kit",
					string_id = "hack_1",
					name = "Hack"
				}
			};
		}

		public override void dumpHackTools() {
			throw new NotImplementedException();
		}

		public override List<Sentry> loadSentries() {
			if (sentries != null)
				return sentries;

			return sentries = new List<Sentry> {
				new Sentry {
					Attacks = new List<Attack> {
						new AttackBasic {
							damage = 2,
							Name = "Nibble",
							Range = 1
						}
					},
					Level = 1,
					MaxHealth = 2,
					Movement = 3,
					string_id = "dog_1",
					description = "No bark, very little bite",
					name = "Guard Pup"
				}
			};
		}

		public override void dumpSentries() {
			throw new NotImplementedException();
		}

		public override List<MapItem> loadMapItems() {
            if (mapItems != null)
                return mapItems;

            return mapItems = new List<MapItem> {
                new MapItem {
                    name = "Wall",
                    string_id = "wall",
                    description = "An impassible threshold"
                },

                new Pickup {
                    name = "Credit",
                    string_id = "credit",
                    description = "You could sell this data for a pretty penny on the market",
                    required = false
                },

                new Pickup {
                    name = "Log Files",
                    string_id = "intel",
                    description = "The information in these files is vital to proceed",
                    required = true
                }
            };
		}

		public override void dumpMapItems() {
			throw new NotImplementedException();
		}

		public override Map loadLevel(string path) {
            if (map != null)
                return map;
            return map = getTestMap();
        }

		public override void dumpMap() {
			throw new NotImplementedException();
		}

        private Map getTestMap() {
            return map = new Map {
                height = 12,
                layers = new List<Layer> {
                    new Layer {
                        height = 12,
                        name = "Geometry",
                        tiles = new[]
                        {
                            new LayerTile {
                                gid = 0
                            },
                            new LayerTile {
                                gid = 1
                            },
                            new LayerTile {
                                gid = 4
                            },
                            new LayerTile {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 1
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 2
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 3
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 3
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 3
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 3
                            },
                            new LayerTile
                            {
                                gid = 5
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            },
                            new LayerTile
                            {
                                gid = 4
                            }
                        },
                        width = 16
                    },
                    new Layer
                    {
                        height = 12,
                        name = "Entities",
                        tiles = new[]
                        {
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 23
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 8
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 22
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 8
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            },
                            new LayerTile
                            {
                                gid = 0
                            }
                        },
                        width = 16
                    }
                },
                tilesets = new List<TileSet> {
                    new TileSet {
                        firstgid = 1,
                        name = "map_features",
                        tiles = new List<Tile> {
                            new Tile {
                                id = 0,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "credit"
                                    }
                                }
                            },
                            new Tile {
                                id = 1,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "intel"
                                    }
                                }
                            },
                            new Tile {
                                id = 2,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "spawnpoint"
                                    }
                                }
                            },
                            new Tile {
                                id = 3,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "path"
                                    }
                                }
                            },
                            new Tile {
                                id = 4,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "wall"
                                    }
                                }
                            }
                        }
                    },
                    new TileSet
                    {
                        firstgid = 6,
                        name = "enemies",
                        tiles = new List<Tile> {
//                            new Tile {
//                                id = 0,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "boss_1"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 1,
//                                properties = new[] {
//                                    new TileSetProperty
//                                    {
//                                        name = "id",
//                                        value = "firewall_1"
//                                    }
//                                }
//                            },
                            new Tile {
                                id = 2,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "dog_1"
                                    }
                                }
                            },
//                            new Tile {
//                                id = 3,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "dog_2"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 4,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "dog_3"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 5,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "sensor_1"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 6,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "sensor_2"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 7,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "sensor_3"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 8,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "sentinel_1"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 9,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "sentinel_2"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 10,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "sentinel_3"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 11,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "warden_1"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 12,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "warden_2"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 13,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "warden_3"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 14,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "watchman_1"
//                                    }
//                                }
//                            },
//                            new Tile {
//                                id = 15,
//                                properties = new[] {
//                                    new TileSetProperty {
//                                        name = "id",
//                                        value = "watchman_2"
//                                    }
//                                }
//                            },
                            new Tile {
                                id = 16,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "watchman_3"
                                    }
                                }
                            },
                            new Tile {
                                id = 17,
                                properties = new[] {
                                    new TileSetProperty {
                                        name = "id",
                                        value = "bug_1"
                                    }
                                }
                            }
                        }
                    }
                },
                width = 16
            };
        }
    }
}