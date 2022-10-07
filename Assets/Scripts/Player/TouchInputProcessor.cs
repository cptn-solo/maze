using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Assets.Scripts
{
    public class TouchInputProcessor : MonoBehaviour
    {
        private const float leftRightRatio = .4f;
        private readonly GUIStyle textStyleLeft = new();
        private readonly GUIStyle textStyleRight = new();
        private Finger leftFinger;
        private Vector2 leftPosPrev = default;
        private Vector2 leftDelta = default;
        
        private Finger rightFinger;
        private Vector2 rightPosPrev = default;
        private Vector2 rightDelta = default;

        public Vector2 LeftDelta => leftDelta;
        public Vector2 RightDelta => rightDelta;


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
            Touch.onFingerUp += Touch_onFingerUp;
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();

            Touch.onFingerDown -= Touch_onFingerDown;
            Touch.onFingerUp -= Touch_onFingerUp;

            leftPosPrev = default;
            leftDelta = default;

            rightPosPrev = default;
            rightDelta = default;
        }
        private void Touch_onFingerDown(Finger obj)
        {
            var pos = obj.currentTouch.startScreenPosition;
            if (pos.x <= Screen.currentResolution.width * leftRightRatio)
            {
                leftFinger = obj;
                leftPosPrev = pos;
                leftDelta = Vector2.zero;
            }
            else
            {
                rightFinger = obj;
                rightPosPrev = pos;
                rightDelta = Vector2.zero;
            }
        }

        private void Touch_onFingerUp(Finger obj)
        {
            if (obj.currentTouch.startScreenPosition.x <= Screen.currentResolution.width * leftRightRatio)
            {
                leftFinger = null;
                leftPosPrev = default;
                leftDelta = default;    
            }
            else
            {
                rightFinger = null;
                rightPosPrev = default;
                rightDelta = default;
            }
        }

        private void Update()
        {
            if (rightFinger != null && rightFinger.isActive &&
                rightFinger.currentTouch is Touch rightTouch &&
                rightTouch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                rightDelta = rightTouch.screenPosition - rightPosPrev;
                rightPosPrev = rightTouch.screenPosition;
            }

            if (leftFinger != null && leftFinger.isActive &&
                leftFinger.currentTouch is Touch leftTouch &&
                leftTouch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                leftDelta = leftTouch.screenPosition - leftPosPrev;
                //leftPosPrev = leftTouch.screenPosition; // for joystick delta is always from the touch start;
           }
        }
    }
}