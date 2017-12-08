using Gamelogic.Extensions;
using Level;
using Level.Entity;
using Models;
using UnityUtilities.Collections.Grid;
using UnityUtilities.Management;

namespace Action.AI {
    class RangedGovernor : Governor {
        public RangedGovernor(LevelModel lm, ILogger logger, SoftwareTool tool) : 
            base(lm, logger, tool) { }
    }
}
