using System.Xml.Serialization;
using Action.AI;
using Zenject;

namespace Level.Entity {
    [XmlType(TypeName = "sentry")]
    public class Sentry : SoftwareTool {
        [Inject]
        public Sentry(SoftwareTool blueprint, GovernorFactory governorFactory, TrailFactory trailFactory) : base(blueprint, governorFactory, trailFactory) {}
        public Sentry() : base() {}

//        public override IEnumerator TakeTurn() {
//            yield return Governor.TakeTurn(this, 0.3f);
//        }
//
//        public IEnumerator TakeTurn(float timePerMove) {
//            yield return Governor.TakeTurn(this, timePerMove);
//        }
//
//        public IEnumerator TakeTurn(Vector2 destination, bool debug) {
//            return Governor.DebugCalculatePath(gridPosition, destination, debug);
//        }
    }
}