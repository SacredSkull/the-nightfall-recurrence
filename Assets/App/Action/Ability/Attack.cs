using System.Xml.Serialization;
using Level.Entity;

namespace Action.Ability {
    public abstract class Attack {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("range")]
        public short Range;

        public abstract bool Execute(SoftwareTool target, SoftwareTool source);

        public Attack() {
            // Overridden by any XML properties specifying range
            Range = 1;
        }
    }
}