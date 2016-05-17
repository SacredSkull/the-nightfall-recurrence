using System.Collections.Generic;
using System.Xml.Serialization;
using Action.Attack;
using UnityEngine;

namespace Level.Entity {
    [XmlType(TypeName = "tool")]
    public class SoftwareTool : Attackable {
        [XmlAttribute("maxsize")]
        public short MaxSize;
        [XmlAttribute("cost")]
        public short Cost;
        [XmlAttribute("level")]
        public short Level;
        [XmlArray("attacks")]
        [XmlArrayItem(ElementName = "attackbasic", Type = typeof(AttackBasic)),
         XmlArrayItem(ElementName = "attributemodifier", Type = typeof(AttributeModifier)),
         XmlArrayItem(ElementName = "mapmodifier", Type = typeof(MapModifier))]
        public List<Attack> Attacks { get; set; }

        public SoftwareTool(){
            gridPosition = new Vector2();
        }

        public bool isEntirelyRanged() {
            bool ranged = false;
            foreach (var attack in Attacks) {
                if (attack.Range > 1)
                    ranged = true;
            }

            return ranged;
        }

        public virtual void Move(Vector2 destination) {
            
        }
    }
}