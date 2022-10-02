﻿using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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

        [SerializeField] private float attachedCameraYOffset = 0.02f;
        [SerializeField] private float attachedCameraMaxDistance = .7f;
        [SerializeField] private float attachedCameraMinDistance = .3f;
        [SerializeField] private float attachedCameraDistance = .5f;
        [SerializeField] private float attachedCameraOrbitSpeed = 90f;

        private Vector3 focusPoint, previousFocusPoint, camPosition, prevCamPosition = default;
        [SerializeField, Min(0f)] private float focusRadius = 5f;
        [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;
        [SerializeField] private float attachedCameraSpeed = .7f;

        private bool isOrbiting;
        private bool isFollowing;
        private bool prevTranslateDirActive;

        private void Awake()
        {
            player = GetComponent<Player>();
            sceneCamera = Camera.main;
            //orbitCamera = sceneCamera.GetComponent<OrbitCamera>();
            //orbitCamera.FocusOn(player.transform);
            
            player.TranslateDirActive = false;
        }

        private void OnEnable()
        {
            if (!listeningForScreenOrientation)
                StartCoroutine(ScreenOrientationMonitor());
        }

        private void OnDisable()
        {
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

            bool lost = camPosition.y != Mathf.Clamp(
                    camPosition.y, focusPoint.y, focusPoint.y + .3f);

            if (notMoved && !isFollowing && !isOrbiting)
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
                var rotX = Quaternion.AngleAxis(cameraAngle, sceneCamera.transform.right);
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
                var pos = sceneCamera.transform.position + step * dir;

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
                
                var yOffset = Mathf.Tan(cameraAngle * Mathf.Deg2Rad) * attachedCameraDistance;

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

            sceneCamera.transform.SetPositionAndRotation(
                camCurrent + camStep, Quaternion.LookRotation(camDirection));
        }

        private void LateUpdate()
        {
            player.TranslateDirActive =
                player.transform.position.y < 2.48f &&
                Mathf.Abs(player.transform.position.x) < 1.5f &&
                Mathf.Abs(player.transform.position.z) < 1.5f;
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
                UpdateFocusPoint();
                PositionCameraBehindPlayer();
            }
            prevTranslateDirActive = player.TranslateDirActive;
        }
        void UpdateFocusPoint()
        {
            previousFocusPoint = focusPoint;
            Vector3 targetPoint = transform.position + Vector3.up * .02f;
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