using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Level.Entity;

namespace Action.Attack {
    public class AttributeModifier : Attack {
        [XmlElement("attribute")]
        public List<Attribute> Attributes;

        public override bool Execute(SoftwareTool target, SoftwareTool source) {
            foreach(Attribute attribute in this.Attributes) {
                if(attribute.name == "movement") {
                    if(attribute.value == 0)
                        target.Movement = 0;
                    else
                        target.Movement += attribute.value;
                } else if(attribute.name == "health") {
                    target.CurrentHealth += attribute.value;
                } else if(attribute.name == "poison") {
                    throw new NotImplementedException();
                }
            }
            return true;

        }
    }
}
