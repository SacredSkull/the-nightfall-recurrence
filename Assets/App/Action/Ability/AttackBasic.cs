using System;
using System.Xml.Serialization;
using Level.Entity;

namespace Action.Ability {
    public class AttackBasic : Attack {
        [XmlAttribute("damage")]
        public short damage;

        public override bool Execute(SoftwareTool target, SoftwareTool source) {
            switch (source) {
                case HackTool _ when target is HackTool:
                    return false;
                case Sentry _ when target is Sentry:
                    throw new ArgumentException("Traitor sentry program detected!");
            }

            if (!target.ReceiveAttack(this, source)) return false;
            return true;

        }
    }
}
