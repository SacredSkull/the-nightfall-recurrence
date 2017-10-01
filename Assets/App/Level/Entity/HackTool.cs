using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

namespace Level.Entity {
    [XmlType(TypeName = "tool")]
    public class HackTool : SoftwareTool {
        public HackTool() : base() {}

        public HackTool(SoftwareTool blueprint) : base(blueprint) {}

        public override IEnumerator TakeTurn() {
            yield return base.TakeTurn();
        }
    }
}
