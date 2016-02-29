using System;
using System.Collections.Generic;
using System.Xml.Serialization;


public class AttributeModifier : Attack {

    [XmlElement("attribute")]
    public List<Attribute> Attributes;

    public override bool attack(SoftwareTool target, SoftwareTool source) {
        foreach(Attribute attribute in this.Attributes) {
            if(attribute.name == "movement") {
                if(attribute.value == 0)
                    target.movement = 0;
                else
                    target.movement += attribute.value;
            } else if(attribute.name == "health") {
                target.health += attribute.value;
            } else if(attribute.name == "poison") {
                throw new NotImplementedException();
            }
        }
        return true;

    }
}
