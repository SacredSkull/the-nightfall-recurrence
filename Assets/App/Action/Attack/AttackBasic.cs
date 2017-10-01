using System;
using System.Xml.Serialization;
using Level.Entity;

namespace Action.Attack {
    public class AttackBasic : Attack {
        [XmlAttribute("damage")]
        public short damage;

        public override bool Execute(SoftwareTool target, SoftwareTool source) {
            if(!(source is Sentry) && !(target is Sentry))
                return false;
            if(source is Sentry && target is Sentry)
                throw new ArgumentException("Traitor sentry program detected!");
            if(target.ReceiveAttack(this, source)) {
                target.Damage(damage);
                return true;
            }

            return false;
        }
    }
}
