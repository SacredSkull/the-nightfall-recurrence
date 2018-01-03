using System.Collections;
using Level.Entity;
using Models;
using ILogger = UnityUtilities.Management.ILogger;

namespace Action.AI {
	public class PlayerGovernor : Governor {
		public PlayerGovernor(LevelModel lm, ILogger _logger, SoftwareTool tool) : base(lm, _logger, tool) { }

		public override IEnumerator TakeTurn(double timePerMove, double turnDelay) {
			yield return null;
		}
	}
}