using System.Xml.Serialization;
using Action.AI;
using Zenject;

namespace Level.Entity {
    [XmlType(TypeName = "tool")]
    public class HackTool : SoftwareTool {
        public HackTool() {}
        
        [Inject]
        public HackTool(SoftwareTool blueprint, GovernorFactory governorFactory, TrailFactory trailFactory) : base(blueprint, governorFactory, trailFactory) {}
    }
}
