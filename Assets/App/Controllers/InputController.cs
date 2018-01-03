using System.Collections.Generic;
using System.Linq;
using Presenters;
using UnityEngine;

namespace Controllers {
    class InputController : MonoBehaviour {
        public bool HoverExitEvent;

        [Range(0.0f, 1.0f)]
        public float Cooldown = 0.001f;
        private float LastTime = 0;

        private readonly HashSet<RaycastHit2D> lastHovered = new HashSet<RaycastHit2D>();
        private bool firstRun = true;

        private void Start() {
            
        }
        
        private void Update() {
            bool onCooldown = (LastTime - Time.time) < Cooldown;
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            // lastHovered will always be empty when the game begins (and until we populate it)
            if(!firstRun && HoverExitEvent) {
                List<RaycastHit2D> temp = new List<RaycastHit2D>();
                foreach(var hit in lastHovered.Except(hits)) {
                    hit.transform.GetComponent<MVCPresenter2D>()?.OnMouseExit();
                    temp.Add(hit);
                }

                foreach(var raycast in temp) {
                    lastHovered.Remove(raycast);
                }
            }
            
            foreach (var hit in hits) {
                MVCPresenter2D presenter = hit.transform.GetComponent<MVCPresenter2D>();
                if(presenter == null)
                    continue;

                if (!onCooldown) {
                    if (Input.GetMouseButtonDown(0)) {
                        presenter.OnMouseDown();
                    }

                    if (Input.GetMouseButtonUp(0)) {
                        presenter.OnMouseUp();
                    }
                }

                lastHovered.Add(hit);

                presenter.OnMouseHovering();
                firstRun = false;
            }
            LastTime = Time.time;
        }
    }
}
