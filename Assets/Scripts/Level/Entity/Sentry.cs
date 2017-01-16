using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Action.Attack;
using Action.Movement;
using UnityEngine;

namespace Level.Entity {
    [XmlType(TypeName = "sentry")]
    public class Sentry : SoftwareTool {
        [XmlAttribute("governor")]
        public string GovernorName;
        [XmlIgnore]
        private Governor _gov;

        public Sentry() : base() {}

        public Sentry(Sentry blueprint) : base(blueprint) {}

        [XmlIgnore]
        public Governor Governor {
            get {
                if(_gov != null) return _gov;

                var assembly = Assembly.GetExecutingAssembly();
                var type = assembly.GetTypes().FirstOrDefault(t => t.Name == GovernorName);
                if (type != null)
                    Governor = Activator.CreateInstance(type) as Governor;
                else
                    Governor = new Governor();
                return _gov;
            }
            set { _gov = value; }
        }

        public override IEnumerator TakeTurn() {
            yield return Governor.TakeTurn(this, 0.3f);
        }

        public IEnumerator TakeTurn(float timePerMove) {
            yield return Governor.TakeTurn(this, timePerMove);
        }

        public IEnumerator TakeTurn(Vector2 destination, bool debug) {
            return Governor.DebugCalculatePath(gridPosition, destination, debug);
        }
    }
}