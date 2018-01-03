using Karma;
using UnityUtilities.Management;
using Zenject;

namespace Presenters {
    public abstract class MVCPresenter2D : MVCPresenter {
        public virtual void OnMouseHovering() { }

        public virtual void OnMouseDown() { }

        public virtual void OnMouseExit() { }

        public virtual void OnMouseEnter() { }

        public virtual void OnMouseUp() { }

        public override void OnPresenterDestroy() { }

        protected ILogger logger;
        
        [Inject]
        protected void SetupLogger(ILogger logger) {
            this.logger = logger;
        }
    }
}
