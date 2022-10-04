﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Assets.Scripts
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float cameraAngle = 18.0f;
        [SerializeField] private float cameraDistance = 1.5f;
        [SerializeField] private float camPortraitFactor = .5f;
        [SerializeField] private float camLandscapeFactor = .75f;
        [SerializeField] private float camSpeed = 10.0f;

        private bool listeningForScreenOrientation;
        private float camDistanceFactor = 1.0f;

        private Player player;
        private Camera sceneCamera;
        //private OrbitCamera orbitCamera;

        [SerializeField] private float attachedCameraAngle = 45.0f;
        [SerializeField] private float attachedCameraMaxDistance = 1.6f;
        [SerializeField] private float attachedCameraMinDistance = .4f;
        [SerializeField] private float attachedCameraDistance = .8f;
        [SerializeField] private float attachedCameraOrbitSpeed = 45f;

        private Vector3 focusPoint, previousFocusPoint, camPosition = default;
        [SerializeField] private float attachedCameraSpeed = .7f;

        private bool isOrbiting;
        private bool isFollowing;
        private bool prevTranslateDirActive;
        
        private PlayerInputActions actions;
        private float cameraSencitivity = .5f;
        private bool cameraControl = true;
        
        private void Awake()
        {
            EnhancedTouchSupport.Enable();

            player = GetComponent<Player>();
            sceneCamera = Camera.main;
            //orbitCamera = sceneCamera.GetComponent<OrbitCamera>();
            //orbitCamera.FocusOn(player.transform);
            
            player.TranslateDirActive = false;

            actions = new PlayerInputActions();
        }
        private void Start()
        {
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = 36;
            textStyle.normal.textColor = Color.green;

            cameraSencitivity = player.Prefs.CameraSencitivity;
            cameraControl = player.Prefs.CameraControl;

            player.Prefs.OnCameraControlChanged += Prefs_OnCameraControlChanged;
            player.Prefs.OnCameraSencitivityChanged += Prefs_OnCameraSencitivityChanged;
        }

        private void OnEnable()
        {
            actions.Enable();
            actions.Default.Camera.performed += Look_performed;
            actions.Default.Camera.canceled += Look_canceled;
            actions.Mobile.Camera.performed += Look_performed;
            actions.Mobile.Camera.canceled += Look_canceled;
            actions.Mobile.RightStick.performed += LookStick_performed;
            actions.Mobile.RightStick.canceled += LookStick_canceled;

            if (!listeningForScreenOrientation)
                StartCoroutine(ScreenOrientationMonitor());
        }

        private void Prefs_OnCameraSencitivityChanged(float obj)
        {
            cameraSencitivity = obj;
            Debug.Log($"Prefs_OnCameraSencitivityChanged {obj}");
        }

        private void Prefs_OnCameraControlChanged(bool obj)
        {
            cameraControl = obj;
            Debug.Log($"Prefs_OnCameraControlChanged {obj}");
        }

        private void Look_canceled(InputAction.CallbackContext obj)
        {
            Debug.Log($"Look_canceled: {obj}");
        }
        Vector2 lookVector = Vector2.zero;
        private bool isStickLooking;
        
        private readonly GUIStyle textStyle = new ();

        private void LookStick_canceled(InputAction.CallbackContext obj)
        {
            Debug.Log($"Look_canceled: {obj}");
            lookVector = Vector2.zero;

            if (isStickLooking)
            {
                isStickLooking = false;
                StopCoroutine(LookWithStick());
            }
        }

        private void LookStick_performed(InputAction.CallbackContext obj)
        {
            if (!cameraControl || player.TranslateDirActive)
                return;

            Debug.Log($"LookStick_performed: {obj}");

            var rawInput = obj.ReadValue<Vector2>();
            lookVector.x = rawInput.x * cameraSencitivity * attachedCameraOrbitSpeed * 2;

            if (!isStickLooking)
                StartCoroutine(LookWithStick());
        }

        private void Look_performed(InputAction.CallbackContext obj)
        {
            if (!cameraControl || player.TranslateDirActive)
                return;

            if (EventSystem.current.IsPointerOverGameObject(obj.control.device.deviceId))
                return;

            Debug.Log($"Look_performed: {obj}");
            
            Look(obj.ReadValue<Vector2>());
        }
        private IEnumerator LookWithStick()
        {
            isStickLooking = true;

            while (isStickLooking && lookVector.x != 0f)
            {
                Look(lookVector);
                yield return null;

            }

            isStickLooking = false;

        }

        private void Look(Vector2 lookVector)
        {
            if (Mathf.Abs(lookVector.x) > 1000)
                return;

            var toCamera = PlaneOffset();
            var angle = lookVector.x * cameraSencitivity;
            var rotY = Quaternion.AngleAxis(angle, transform.up);
            var step = rotY * toCamera.normalized;

            var yOffset = Mathf.Tan(attachedCameraAngle * Mathf.Deg2Rad) * attachedCameraDistance;

            var pos = focusPoint +
                step * toCamera.magnitude +
                yOffset * transform.up;

            camPosition = pos;
            
            sceneCamera.transform.position = camPosition;

        }

        private void OnDisable()
        {
            actions.Disable();
            actions.Default.Camera.performed -= Look_performed;
            actions.Default.Camera.canceled -= Look_canceled;
            actions.Mobile.Camera.performed += Look_performed;
            actions.Mobile.Camera.canceled += Look_canceled;

            if (listeningForScreenOrientation)
                StopCoroutine(ScreenOrientationMonitor());
        }

        private IEnumerator ScreenOrientationMonitor()
        {
            listeningForScreenOrientation = true;
            while (listeningForScreenOrientation)
            {
                var factor = Screen.orientation switch
                {
                    ScreenOrientation.Portrait => camPortraitFactor,
                    ScreenOrientation.PortraitUpsideDown => camPortraitFactor,
                    _ => camLandscapeFactor
                };
                camDistanceFactor = ((float)Screen.height) * factor / Screen.width;

                yield return new WaitForSecondsRealtime(1.0f);
            }
        }

        private void PositionCameraBehindPlayer()
        {
            if (camPosition.Equals(default))
            {
                Focus();
                return;
            }
            
            bool notMoved = 
                !previousFocusPoint.Equals(default) && 
                !focusPoint.Equals(default) &&
                (previousFocusPoint - focusPoint).sqrMagnitude < 0.00001f;

            var toCamera = PlaneOffset();
            var distance = toCamera.magnitude;

            bool tooClose = distance < attachedCameraMinDistance;
            
            bool tooFar = distance > attachedCameraMaxDistance;
            
            var yOffset = Mathf.Tan(attachedCameraAngle * Mathf.Deg2Rad) * attachedCameraDistance;

            bool lost = camPosition.y != Mathf.Clamp(
                    camPosition.y, focusPoint.y, focusPoint.y + yOffset + .1f);

            if (!cameraControl && notMoved && !isFollowing && !isOrbiting)
                StartCoroutine(OrbitCoroutine());

            if (tooClose || tooFar)
            {
                if (isOrbiting)
                {
                    isOrbiting = false;
                    StopCoroutine(OrbitCoroutine());
                }
                if (!isFollowing)
                    StartCoroutine(FollowCoroutine());
            }

            if (lost)
            {
                isOrbiting = false;
                isFollowing = false;
                StopCoroutine(OrbitCoroutine());
                StopCoroutine(FollowCoroutine());

                Focus();
            }

            sceneCamera.transform.LookAt(focusPoint);

            void Focus()
            {
                var rotX = Quaternion.AngleAxis(attachedCameraAngle, sceneCamera.transform.right);
                var pos = focusPoint + rotX * -transform.forward * attachedCameraDistance;

                camPosition = pos;

                sceneCamera.transform.position = camPosition;
            }
        }
        private IEnumerator FollowCoroutine()
        {
            isFollowing = true;
                        
            while (isFollowing)
            {
                var toCamera = PlaneOffset();
                var distance = toCamera.magnitude;
                var deltaDistance = distance - attachedCameraDistance;
                var sign = -Mathf.Sign(deltaDistance);
                var step = Mathf.Min(
                    attachedCameraSpeed * Time.deltaTime, 
                    Mathf.Abs(deltaDistance));

                var dir = toCamera.normalized * sign;

                var yOffset = Mathf.Tan(attachedCameraAngle * Mathf.Deg2Rad) *
                    attachedCameraDistance;

                var pos = sceneCamera.transform.position + step * dir;
                pos.y = focusPoint.y + yOffset;

                camPosition = pos;

                sceneCamera.transform.position = camPosition;

                if (step == Mathf.Abs(deltaDistance))
                    break;

                yield return null;
            }

            isFollowing = false;
        }

        private IEnumerator OrbitCoroutine(Vector3 alignTo = default)
        {
            if (alignTo.Equals(default))
                alignTo = -transform.forward;

            isOrbiting = true;

            var angle = TrailAngle();
            var timeAtSpeed = Mathf.Abs(angle) / attachedCameraOrbitSpeed;
            var elapsedTime = 0f;

            while (elapsedTime < timeAtSpeed)
            {
                if (previousFocusPoint != focusPoint)
                    break;

                var toCamera = PlaneOffset();
                var step = Vector3.Slerp(toCamera.normalized, alignTo, 
                    elapsedTime / timeAtSpeed);
                
                var yOffset = Mathf.Tan(attachedCameraAngle * Mathf.Deg2Rad) * 
                    attachedCameraDistance;

                var pos = focusPoint + 
                    step * toCamera.magnitude + 
                    yOffset * transform.up;
                
                camPosition = pos;

                sceneCamera.transform.position = camPosition;

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            isOrbiting = false;
        }
        
        private void PositionCameraTowardsCenter()
        {
            var buildingFloorY = Vector3.zero + Vector3.up * player.transform.position.y;
            var buildingToPlayer = Vector3.Distance(player.transform.position, buildingFloorY);

            var yOffset = Mathf.Tan(cameraAngle * Mathf.Deg2Rad) * buildingToPlayer;

            var cameraRay = new Ray(
                buildingFloorY,
                (player.transform.position + player.transform.up * yOffset - buildingFloorY).normalized);

            var camCurrent = sceneCamera.transform.position;

            var camPosition = player.transform.position + camDistanceFactor * cameraDistance * cameraRay.direction;
            var camDirection = player.transform.position - camPosition;

            var camStep = camSpeed * Time.deltaTime * (camPosition - camCurrent);

            if (!camDirection.Equals(Vector3.zero))
                sceneCamera.transform.SetPositionAndRotation(
                    camCurrent + camStep, Quaternion.LookRotation(camDirection));
        }
        private void OnGUI()
        {
            if (!EnhancedTouchSupport.enabled)
                return;

            GUI.Label(new Rect(42, 300, 100, 40),
                $"F:{Touch.activeFingers.Count} T:{Touch.activeTouches.Count}",
                textStyle);

            for (int i = 0; i < Touch.activeTouches.Count; i++)
            {
                GUI.Label(new Rect(42, 350 + (50 * i), 100, 40),
                    $"T:{i} D:{Touch.activeTouches[i].delta}",
                    textStyle);
            }
        }


        private void LateUpdate()
        {
            player.TranslateDirActive =
                player.transform.position.y < 2.48f &&
                Mathf.Abs(player.transform.position.x) < 1.25f &&
                Mathf.Abs(player.transform.position.z) < 1.25f;
            if (player.TranslateDirActive)
            {
                if (!prevTranslateDirActive)
                {
                    isOrbiting = false;
                    isFollowing = false;
                    StopCoroutine(OrbitCoroutine());
                    StopCoroutine(FollowCoroutine());
                }
                PositionCameraTowardsCenter();
            }
            else {

                if (!prevTranslateDirActive)
                {

                }
                
                UpdateFocusPoint();
                PositionCameraBehindPlayer();
            }
            prevTranslateDirActive = player.TranslateDirActive;
        }
        void UpdateFocusPoint()
        {
            previousFocusPoint = focusPoint;
            Vector3 targetPoint = transform.position + Vector3.up * .08f;
            focusPoint = targetPoint;
        }

        private Vector3 PlaneOffset()
        {
            return Vector3.ProjectOnPlane(
                sceneCamera.transform.position - focusPoint, transform.up);
        }
        private float TrailAngle()
        {
            var toCamera = PlaneOffset();
            var angle = Vector3.SignedAngle(
                toCamera, -transform.forward,
                transform.up);

            return angle;
        }
    }
}