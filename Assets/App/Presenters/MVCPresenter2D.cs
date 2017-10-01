using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Karma;

namespace Presenters {
    public class MVCPresenter2D : MVCPresenter {
        public virtual void OnMouseHovering() { }

        public virtual void OnMouseDown() { }

        public virtual void OnMouseExit() { }

        public virtual void OnMouseEnter() { }

        public virtual void OnMouseUp() { }

        public override void OnPresenterDestroy() { }
    }
}
