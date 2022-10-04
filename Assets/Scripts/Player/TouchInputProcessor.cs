using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Assets.Scripts
{
    public class TouchInputProcessor : MonoBehaviour
    {
        public event Action<Vector2> OnLeftTouchBegin;
        public event Action<Vector2> OnLeftTouchMove;
        public event Action<Vector2> OnLeftTouchEnd;

        public event Action<Vector2> OnRightTouchBegin;
        public event Action<Vector2> OnRightTouchMove;
        public event Action<Vector2> OnRightTouchEnd;

        private readonly GUIStyle textStyleLeft = new();
        private readonly GUIStyle textStyleRight = new();
        
        private Finger leftFinger = null;
        private Finger rightFinger = null;

        private Vector2 leftDelta = default;
        private Vector2 rightDelta = default;

        private void Awake()
        {
            textStyleLeft.fontStyle = FontStyle.Bold;
            textStyleLeft.fontSize = 36;
            textStyleLeft.normal.textColor = Color.blue;

            textStyleRight.fontStyle = FontStyle.Bold;
            textStyleRight.fontSize = 36;
            textStyleRight.normal.textColor = Color.red;
        }
        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();

            Touch.onFingerDown += Touch_onFingerDown;
            Touch.onFingerMove += Touch_onFingerMove;
            Touch.onFingerUp += Touch_onFingerUp;
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();

            Touch.onFingerDown -= Touch_onFingerDown;
            Touch.onFingerMove -= Touch_onFingerMove;
            Touch.onFingerUp -= Touch_onFingerUp;
        }
        private void Touch_onFingerUp(Finger obj)
        {
            if (obj.screenPosition.x < Screen.currentResolution.width * .5f)
            {
                leftFinger = null;
                leftDelta = default;
            }
            else
            {
                rightFinger = null;
                rightDelta = default;
            }
        }

        private void Touch_onFingerMove(Finger obj)
        {
            var delta = obj.currentTouch.delta;
            if (leftFinger != null && leftFinger.index == obj.index)
                leftDelta = delta;
            else
                rightDelta = delta;
        }

        private void Touch_onFingerDown(Finger obj)
        {
            if (obj.screenPosition.x < Screen.currentResolution.width * .5f)
            {
                leftFinger = obj;
                leftDelta = Vector2.zero;
            }
            else
            {
                rightFinger = obj;
                rightDelta = Vector2.zero;
            }
        }

        private void Update()
        {            
        }
        private void OnGUI()
        {
            if (!EnhancedTouchSupport.enabled)
                return;

            GUI.Label(new Rect(42, 300, 100, 40),
                $"F:{Touch.activeFingers.Count} T:{Touch.activeTouches.Count}",
                textStyleLeft);

            if (leftDelta != default)
                GUI.Label(new Rect(42, 350, 100, 40),
                    $"{leftDelta}",
                    textStyleLeft);

            if (rightDelta != default)
                GUI.Label(new Rect(42, 400, 100, 40),
                    $"{rightDelta}",
                    textStyleRight);
        }


    }
}