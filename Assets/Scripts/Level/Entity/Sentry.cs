using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Action.Movement;
using UnityEngine;

namespace Level.Entity {
    [XmlType(TypeName = "sentry")]
    public class Sentry : SoftwareTool {
        [XmlAttribute("governor")]
        public string GovernorName;
        [XmlIgnore]
        private Governor _gov;
        [XmlIgnore]
        public Governor Governor {
            get {
                if (_gov == null) {
                    var assembly = Assembly.GetExecutingAssembly();
                    var type = assembly.GetTypes().FirstOrDefault(t => t.Name == GovernorName);
                    if (type != null)
                        Governor = Activator.CreateInstance(type) as Governor;
                    else
                        Governor = new Governor();
                }
                return _gov;
            }
            set { _gov = value; }
        }

        public override void Move(Vector2 destination) {
            Governor.Move(destination, this);
        }

        public IEnumerator Move(Vector2 destination, bool debug) {
            return Governor.DebugCalculatePath(gridPosition, destination, debug);
        }
    }
}