using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Views;

namespace Controllers {
    class InputController : MonoBehaviour {
        public bool HoverExitEvent;

        private readonly HashSet<RaycastHit2D> lastHovered = new HashSet<RaycastHit2D>();
        private bool firstRun = true;

        private void Update() {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            // lastHovered will always be empty when the game begins (and until we populate it)
            if(!firstRun && HoverExitEvent) {
                List<RaycastHit2D> temp = new List<RaycastHit2D>();
                foreach(var hit in lastHovered.Except(hits)) {
                    hit.transform.GetComponent<View>()?.MouseExit();
                    temp.Add(hit);
                }

                foreach(var raycast in temp) {
                    lastHovered.Remove(raycast);
                }
            }

            foreach (var hit in hits) {
                View view = hit.transform.GetComponent<View>();
                if(view == null)
                    continue;

                if(Input.GetMouseButtonDown(0)) {
                    view.MouseDown();
                }

                if(Input.GetMouseButtonUp(0)) {
                    view.MouseUp();
                }

                lastHovered.Add(hit);

                view.MouseHovering();
                firstRun = false;
            }
        }
    }
}
