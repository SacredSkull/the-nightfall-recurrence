#if !NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    // Note: this corresponds to the values expected in
    // Input.GetMouseButtonDown() and similar methods
    public enum MouseButtons
    {
        None,
        Left,
        Right,
        Middle,
    }

    [System.Diagnostics.DebuggerStepThrough]
    public class UnityEventManager : MonoBehaviour, ITickable
    {
        public event System.Action ApplicationGainedFocus = delegate { };
        public event System.Action ApplicationLostFocus = delegate { };
        public event Action<bool> ApplicationFocusChanged = delegate { };
        public event System.Action ApplicationQuit = delegate { };
        public event System.Action ChangingScenes = delegate { };
        public event System.Action DrawGizmos = delegate { };
        public event Action<MouseButtons> MouseButtonDown = delegate { };
        public event Action<MouseButtons> MouseButtonUp = delegate { };
        public event System.Action LeftMouseButtonDown = delegate { };
        public event System.Action LeftMouseButtonUp = delegate { };
        public event System.Action MiddleMouseButtonDown = delegate { };
        public event System.Action MiddleMouseButtonUp = delegate { };
        public event System.Action RightMouseButtonDown = delegate { };
        public event System.Action RightMouseButtonUp = delegate { };
        public event System.Action MouseMoved = delegate { };
        public event System.Action ScreenSizeChanged = delegate { };
        public event System.Action Started = delegate { };
        public event Action<float> MouseWheelMoved = delegate { };

        Vector3 _lastMousePosition;

        int _lastWidth;
        int _lastHeight;

        public bool IsFocused
        {
            get;
            private set;
        }

        void Start()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            Started();
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown((int)MouseButtons.Left))
            {
                LeftMouseButtonDown();
                MouseButtonDown(MouseButtons.Left);
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Left))
            {
                LeftMouseButtonUp();
                MouseButtonUp(MouseButtons.Left);
            }

            if (Input.GetMouseButtonDown((int)MouseButtons.Right))
            {
                RightMouseButtonDown();
                MouseButtonDown(MouseButtons.Right);
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Right))
            {
                RightMouseButtonUp();
                MouseButtonUp(MouseButtons.Right);
            }

            if (Input.GetMouseButtonDown((int)MouseButtons.Middle))
            {
                MiddleMouseButtonDown();
                MouseButtonDown(MouseButtons.Middle);
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Middle))
            {
                MiddleMouseButtonUp();
                MouseButtonUp(MouseButtons.Middle);
            }

            if (_lastMousePosition != Input.mousePosition)
            {
                _lastMousePosition = Input.mousePosition;
                MouseMoved();
            }

            // By default this event returns 1/10 for each discrete rotation
            // so correct that
            var mouseWheelDelta = 10.0f * Input.GetAxis("Mouse ScrollWheel");

            if (!Mathf.Approximately(mouseWheelDelta, 0))
            {
                MouseWheelMoved(mouseWheelDelta);
            }

            if (_lastWidth != Screen.width || _lastHeight != Screen.height)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
                ScreenSizeChanged();
            }
        }

        void OnDestroy()
        {
            ChangingScenes();
        }

        void OnApplicationQuit()
        {
            ApplicationQuit();
        }

        void OnDrawGizmos()
        {
            DrawGizmos();
        }

        void OnApplicationFocus(bool newIsFocused)
        {
            if (newIsFocused && !IsFocused)
            {
                IsFocused = true;
                ApplicationGainedFocus();
                ApplicationFocusChanged(true);
            }

            if (!newIsFocused && IsFocused)
            {
                IsFocused = false;
                ApplicationLostFocus();
                ApplicationFocusChanged(false);
            }
        }
    }
}

#endif
