using System.Collections;
using Action.Ability;
using Karma;
using Level.Entity;
using UnityEngine;

namespace Presenters {
    class AnimationView : MVCPresenter {
        public void Awake() {
            SoftwareTool.DeathEvent += (victim, killer, candlestick) => {
                StartCoroutine(DestroyedTool(victim, killer, candlestick));
            };
        }

        private IEnumerator DestroyedTool(SoftwareTool victim, SoftwareTool killer, Attack candlestick) {
            yield return new WaitForSeconds(1f);
        }

        public override void OnPresenterDestroy() {

        }
    }
}
