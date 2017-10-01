using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Level;
using Level.Entity;
using UnityEngine;
using Utility;
using Utility.Collections.Grid;
using Logger = UnityUtilities.Logger;

namespace Level.Entity {
    public class TrailTile : MapItem {
        public Trail Trail { get; private set; }

        public TrailTile(Trail trail) {
            Trail = trail;
        }

        public TrailTile(TrailTile template) : base(template) {
            Trail = template.Trail;
        }
    }

    public class Trail : IEnumerable<TrailTile> {
        private List<TrailTile> Tail = new List<TrailTile>();
        public SoftwareTool Head;
        private TrailTile segmentTemplate;

        public Trail(SoftwareTool head) {
            Head = head;
            segmentTemplate = new TrailTile(this) {
                description = $"Allocated memory belonging to {Head.name}",
                name = $"{Head.name}'s memory sectors",
                sprite = Head.TailSprite
            };
        }

        public void Move() {
            // TODO: Check for overlap - and shorten to compensate
            if(Head.MaxHealth <= 1) return;

            if(Head.CurrentHealth > 1) {
                if(Head.CurrentHealth >= 2) {
                    TrailTile overlappedTrail = Tail.FirstOrDefault(x => x.GetPosition() == Head.GetPosition());
                    if(overlappedTrail != null) {
                        overlappedTrail.SetPosition(Head.PreviousPosition, false);
                        return;
                    }
                    for(int i = 0; i < Tail.Count; i++) {
                        if (Head.GetPosition() == Tail[i].GetPosition()) {
                            if(i == 0) {
                                Tail[0].SetPosition(Head.PreviousPosition);
                            }
                            return;
                        }

                        if(i == 0) {
                            Tail[0].SetPosition(Head.PreviousPosition);
                            continue;
                        }

                        Tail[i].SetPosition(Tail[i - 1].PreviousPosition);
                    }
                }

                if (Head.CurrentHealth >= Head.MaxHealth) return;

                Lengthen(VectorList.Contains(Head.GetPosition()) ? Tail.Last().PreviousPosition : Head.PreviousPosition);
            } else {
                Lengthen(Head.PreviousPosition);
            }
        }

        public void Lengthen(Vector2 pos) {
            Head.CurrentHealth++;
            TrailTile segment = new TrailTile(segmentTemplate);
            if(Tail.Count != 0) {
                pos = Tail.Last().PreviousPosition;
            }
            Tail.Add(segment);
            segment.SetPosition(pos);
            ServiceLocator.GetLevelEntityGrid().Set(pos, 0, segment);
        }

        public void Shorten() {
            Head.CurrentHealth--;
            TrailTile tile = Tail.Last();
            Tail.Remove(tile);
            tile.Delete();
        }

        public Vector2[] VectorList => Tail.Select(x => x.GetPosition()).ToArray();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<TrailTile> GetEnumerator() {
            return Tail.GetEnumerator();
        }

        public int Count => Tail.Count;
    }
}
