using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action.Attack;
using Controllers;
using Level.Entity;
using thelab.mvc;
using UnityEngine;

namespace Assets.Scripts.Views {
    class AnimationView : View<ActiveLevel> {
        public void Awake() {
            SoftwareTool.DeathEvent += (victim, killer, candlestick) => {
                StartCoroutine(DestroyedTool(victim, killer, candlestick));
            };
        }

        private IEnumerator DestroyedTool(SoftwareTool victim, SoftwareTool killer, Attack candlestick) {
            yield return new WaitForSeconds(1f);
        }
    }
}
