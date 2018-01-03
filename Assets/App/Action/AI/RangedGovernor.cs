using Level.Entity;
using Models;
using UnityUtilities.Management;

namespace Action.AI {
    class RangedGovernor : Governor {
        public RangedGovernor(LevelModel lm, ILogger logger, SoftwareTool tool) : 
            base(lm, logger, tool) { }
    }
}
