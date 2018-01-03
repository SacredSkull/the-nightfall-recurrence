using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Level.Entity;

namespace Action.Ability {
    public sealed class StatusEffectOverallType {
        private static Dictionary<string, StatusEffectOverallType> dict = new Dictionary<string, StatusEffectOverallType>(); 
        private readonly string name;
        private readonly int value;

        public static readonly StatusEffectOverallType BUFF_SUM = new StatusEffectOverallType (1, "BUFF-SUM");
        public static readonly StatusEffectOverallType BUFF_SET = new StatusEffectOverallType (2, "BUFF-SET");

        private StatusEffectOverallType(int value, string name){
            this.name = name;
            this.value = value;
            dict[name] = this;
        }

        public override string ToString(){
            return name;
        }
        
        public static StatusEffectOverallType Find(string name) {
            return dict.First(x => x.Key == name.ToUpper()).Value;
        }
    }

    public sealed class AttributeTargetType {
        private static Dictionary<string, AttributeTargetType> dict = new Dictionary<string, AttributeTargetType>(); 
        private readonly string name;
        private readonly int value;

        public static readonly AttributeTargetType MOVEMENT = new AttributeTargetType (1, "MOVEMENT");
        public static readonly AttributeTargetType HEALTH = new AttributeTargetType (2, "HEALTH");
        public static readonly AttributeTargetType ABILITIES = new AttributeTargetType (3, "ABILITIES");
        public static readonly AttributeTargetType ACTIVE = new AttributeTargetType (4, "ACTIVE");

        private AttributeTargetType(int value, string name){
            this.name = name;
            this.value = value;
            dict[name] = this;
        }

        public override string ToString(){
            return name;
        }
        
        public static AttributeTargetType Find(string name) {
            return dict.First(x => x.Key == name.ToUpper()).Value;
        }
    }
    
    public class AttributeModifier : Attack {
        [XmlElement("attribute")]
        public List<Attribute> Attributes;

        public override bool Execute(SoftwareTool target, SoftwareTool source) {
            if(target.StatusEffects == null)
                target.StatusEffects = new HashSet<TemporalStatusEffect>();
            
            foreach(Attribute attribute in Attributes) {
                TemporalStatusEffect effect = new TemporalStatusEffect(Name, target, source, StatusEffectOverallType.Find(attribute.operation), AttributeTargetType.Find(attribute.name), attribute.value, attribute.turnCount);
                
                TemporalStatusEffect existing = target.StatusEffects.FirstOrDefault(x => x.Equals(effect));
                if (existing != null) {
                    existing.Counter = attribute.turnCount;
                    continue;
                }

                target.StatusEffects.Add(effect);
                //TODO: Add status effect irregardless if the operator is sum or set (will be summed)

            }
            return true;

        }
    }
}
