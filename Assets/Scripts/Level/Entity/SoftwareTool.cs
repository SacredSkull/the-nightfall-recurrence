using System.Collections.Generic;
using System.Xml.Serialization;
using Action.Attack;
using UnityEngine;

namespace Level.Entity {
    //TODO: Add the sound clip for movement in this class. It should be activated inside the move() function, which will also need a reference to the AudioQueue (via the ServiceLocator), which hasn't been made yet!
    //renderedTile.GetComponent<AudioSource>().clip = MovementClip;
    public abstract class SoftwareTool : MapItem {
        [XmlIgnore]
        public int CurrentHealth = 1;
        [XmlAttribute("maxsize")]
        public int MaxHealth;
        [XmlAttribute("cost")]
        public short Cost;
        [XmlAttribute("level")]
        public short Level;
        [XmlAttribute("movement")]
        public int Movement { get; set; }
        [XmlArray("attacks")]
        [XmlArrayItem(ElementName = "attackbasic", Type = typeof(AttackBasic)),
         XmlArrayItem(ElementName = "attributemodifier", Type = typeof(AttributeModifier)),
         XmlArrayItem(ElementName = "mapmodifier", Type = typeof(MapModifier))]
        public List<Attack> Attacks { get; set; }

        public SoftwareTool(){
            gridPosition = new Vector2();
        }

        public SoftwareTool(SoftwareTool blueprint) : base(blueprint) {
            MaxHealth = blueprint.MaxHealth;
            Cost = blueprint.Cost;
            Level = blueprint.Level;
            CurrentHealth = blueprint.CurrentHealth;
            Movement = blueprint.Movement;
            Attacks = blueprint.Attacks;
        }

        public bool isEntirelyRanged() {
            bool ranged = false;
            foreach (var attack in Attacks) {
                if (attack.Range > 1)
                    ranged = true;
            }

            return ranged;
        }

        public virtual void TakeTurn() {
            
        }

        public virtual void Move(Vector2 destination) {
            
        }
    }
}