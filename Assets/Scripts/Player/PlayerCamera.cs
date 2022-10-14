using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float cameraAngle = 18.0f;
        [SerializeField] private float cameraDistance = 1.5f;
        [SerializeField] private float camPortraitFactor = .5f;
        [SerializeField] private float camLandscapeFactor = .75f;
        [SerializeField] private float camSpeed = 10.0f;

        [SerializeField] private LayerMask obstructionMask;

        private bool listeningForScreenOrientation;
        private float camDistanceFactor = 1.0f;

        private Player player;
        private TouchInputProcessor touches;
        private Camera sceneCamera;

        public void AttachCamera(Camera cam) =>
            sceneCamera = cam;

        [SerializeField] private float attachedCameraAngle = 45.0f;
        [SerializeField] private float attachedCameraMaxDistance = 1.6f;
        [SerializeField] private float attachedCameraMinDistance = .4f;
        [SerializeField] private float attachedCameraDistance = .8f;
        [SerializeField] private float attachedCameraOrbitSpeed = 45f;

        private Vector3 focusPoint, previousFocusPoint, camPosition = default;
        [SerializeField] private float attachedCameraSpeed = .7f;

        private bool isOrbiting;
        private bool isFollowing;
        private bool obscured;
        private bool cameraMovePause;

        public float CameraSencitivity { get; set; } = .5f;
        public bool CameraControl { get; set; } = true;

        private void Awake()
        {
            player = GetComponent<Player>();
            touches = GetComponent<TouchInputProcessor>();
        }

        private void OnEnable()
        {
            player.OnTranslateDirActiveChange += Player_OnTranslateDirActiveChange;
            player.OnBeforeFadeOut += Player_OnBeforeFadeOut;
            player.OnAfterFadeIn += Player_OnAfterFadeIn;

            if (!listeningForScreenOrientation)
                StartCoroutine(ScreenOrientationMonitor());
        }

        private void OnDisable()
        {
            player.OnTranslateDirActiveChange -= Player_OnTranslateDirActiveChange;
            player.OnBeforeFadeOut -= Player_OnBeforeFadeOut;
            player.OnAfterFadeIn -= Player_OnAfterFadeIn;

            if (listeningForScreenOrientation)
                StopCoroutine(ScreenOrientationMonitor());
        }
        private void Update()
        {
            player.ToggleTranslateDir();
        }
        private void LateUpdate()
        {
            if (sceneCamera == null)
                return;

            if (player.TranslateDirActive)
            {
                PositionCameraTowardsCenter();
            }
            else
            {
                if (touches.RightDelta is Vector2 delta && delta != default)
                    Look(delta);

                UpdateFocusPoint();
                PositionCameraBehindPlayer();
            }
        }

        private void Player_OnAfterFadeIn(MovableUnit obj)
        {
            if (!player.TranslateDirActive)
                Focus();

            cameraMovePause = false;
        }

        private void Player_OnBeforeFadeOut(MovableUnit obj)
        {
            isFollowing = false;
            StopCoroutine(FollowCoroutine());
            isOrbiting = false;
            StopCoroutine(OrbitCoroutine());
            cameraMovePause = true;
        }

        private void Player_OnTranslateDirActiveChange(bool obj)
        {
            isOrbiting = false;
            isFollowing = false;
            StopCoroutine(OrbitCoroutine());
            StopCoroutine(FollowCoroutine());
        }

        void Focus()
        {
            var rotX = Quaternion.AngleAxis(attachedCameraAngle, sceneCamera.transform.right);
            var pos = focusPoint + rotX * -transform.forward * attachedCameraDistance;

            camPosition = AvoidObstacles(pos);

            sceneCamera.transform.position = camPosition;
        }

        private void Look(Vector2 lookVector)
        {
            if (!CameraControl)
                return;

            if (lookVector == default)
                return;

            if (float.IsInfinity(lookVector.x) ||
                float.IsNaN(lookVector.x))
                return;

            var toCamera = PlaneOffset();
            var angle = -lookVector.x * CameraSencitivity * .05f;
            var rotY = Quaternion.AngleAxis(angle, transform.up);
            var step = rotY * toCamera.normalized;

            var yOffset = Mathf.Tan(attachedCameraAngle * Mathf.Deg2Rad) * attachedCameraDistance;

            var pos = focusPoint +
                step * toCamera.magnitude +
                yOffset * transform.up;

            camPosition = AvoidObstacles(pos);

            sceneCamera.transform.position = camPosition;
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
                    camPosition.y, focusPoint.y + yOffset - .1f, focusPoint.y + yOffset + .1f);

            if (!CameraControl && notMoved && !isFollowing && !isOrbiting)
                StartCoroutine(OrbitCoroutine());

            if ((!obscured || !notMoved) && (tooClose || tooFar || lost))
            {
                if (isOrbiting)
                {
                    isOrbiting = false;
                    StopCoroutine(OrbitCoroutine());
                }
                if (!isFollowing)
                    StartCoroutine(FollowCoroutine());
            }
            if (!notMoved)
                StrafeAfterMove();

            sceneCamera.transform.LookAt(focusPoint);

        }

        private void StrafeAfterMove()
        {
            var toCamera = PlaneOffset();
            var cameraPlaneForward = Vector3.ProjectOnPlane(
                sceneCamera.transform.forward, Vector3.up);
            var xAngle = Vector3.SignedAngle(cameraPlaneForward.normalized, - toCamera.normalized, Vector3.up);
                                    
            var xOffset = Mathf.Tan(xAngle * Mathf.Deg2Rad) * toCamera.magnitude;

            sceneCamera.transform.position += sceneCamera.transform.right * xOffset;
        }

        private IEnumerator FollowCoroutine()
        {
            isFollowing = true;

            var optimalDistance = attachedCameraDistance;

            while (isFollowing)
            {
                var toCamera = PlaneOffset();
                var distance = toCamera.magnitude;
                var deltaDistance = distance - optimalDistance;
                var sign = -Mathf.Sign(deltaDistance);
                var step = Mathf.Min(
                    attachedCameraSpeed * Time.deltaTime, 
                    Mathf.Abs(deltaDistance));

                var dir = toCamera.normalized * sign;

                var yOffset = Mathf.Tan(attachedCameraAngle * Mathf.Deg2Rad) *
                    optimalDistance;

                var pos = sceneCamera.transform.position + step * dir;
                pos.y = focusPoint.y + yOffset;
                
                camPosition = AvoidObstacles(pos);

                sceneCamera.transform.position = camPosition;

                if (obscured || step == Mathf.Abs(deltaDistance))
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

                camPosition = AvoidObstacles(pos);

                sceneCamera.transform.position = camPosition;

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            isOrbiting = false;
        }
        
        private void PositionCameraTowardsCenter()
        {
            if (cameraMovePause)
                return;

            var buildingFloorY = Vector3.zero + Vector3.up * player.transform.position.y;
            var toPlayer = (player.transform.position - buildingFloorY).normalized;
            var rotX = Quaternion.AngleAxis(-cameraAngle, Quaternion.AngleAxis(90f, Vector3.up) * toPlayer);

            var cameraRay = new Ray(buildingFloorY, rotX * toPlayer);

            var camCurrent = sceneCamera.transform.position;

            var camPosition = player.transform.position + camDistanceFactor * cameraDistance * cameraRay.direction;
            var camDirection = player.transform.position - camPosition;

            var camStep = camSpeed * Time.deltaTime * (camPosition - camCurrent);

            if (!camDirection.Equals(Vector3.zero))
                sceneCamera.transform.SetPositionAndRotation(
                    camCurrent + camStep, Quaternion.LookRotation(camDirection));
        }
        
        void UpdateFocusPoint()
        {
            previousFocusPoint = focusPoint;
            Vector3 targetPoint = transform.position + Vector3.up * .14f;
            focusPoint = targetPoint;
        }

        private Vector3 PlaneOffset(Vector3 pos = default)
        {
            if (pos == default)
                pos = sceneCamera.transform.position;
            return Vector3.ProjectOnPlane(
                pos - focusPoint, transform.up);
        }
        private float TrailAngle()
        {
            var toCamera = PlaneOffset();
            var angle = Vector3.SignedAngle(
                toCamera, -transform.forward,
                transform.up);

            return angle;
        }
        private Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                halfExtends.y =
                    sceneCamera.nearClipPlane *
                    Mathf.Tan(0.5f * Mathf.Deg2Rad * sceneCamera.fieldOfView);
                halfExtends.x = halfExtends.y * sceneCamera.aspect;
                halfExtends.z = 0f;
                return halfExtends;
            }
        }
        private Vector3 AvoidObstacles(Vector3 pos)
        {
            return pos;

            Vector3 lookPosition = pos;
            Vector3 lookDirection = (focusPoint - lookPosition).normalized;
            Vector3 rectOffset = lookDirection * sceneCamera.nearClipPlane;
            Vector3 rectPosition = lookPosition + rectOffset;
            Vector3 castFrom = focusPoint;
            Vector3 castLine = rectPosition - castFrom;
            float castDistance = castLine.magnitude;
            Vector3 castDirection = castLine / castDistance;

            Quaternion lookOrientation = Quaternion.LookRotation(lookDirection);
            
            obscured = false;

            if (Physics.BoxCast(
                castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
                lookOrientation, castDistance, obstructionMask
            ))
            {
                obscured = true;
                rectPosition = castFrom + castDirection * hit.distance;
                return rectPosition - rectOffset;
            }
            return pos;
        }
    }
}