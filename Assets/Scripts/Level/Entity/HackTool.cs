using System.Xml.Serialization;
using UnityEngine;

namespace Level.Entity {
    [XmlType(TypeName = "tool")]
    public class HackTool : SoftwareTool {
        public HackTool() {}

        public HackTool(SoftwareTool blueprint) : base(blueprint) {}

        public override void TakeTurn() {
            base.TakeTurn();
        }

        public override void Move(Vector2 destination) {
            base.Move(destination);
        }
    }
}
