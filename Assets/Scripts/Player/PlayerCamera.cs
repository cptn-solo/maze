using System.Collections;
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
        [SerializeField] private float attachedCameraZOffset = -0.15f;
        [SerializeField] private float attachedCameraFollowDelay = 1f;
        [SerializeField] private float attachedCameraMaxDistance = .7f;
        [SerializeField] private float attachedCameraMinDistance = .3f;
        [SerializeField] private float attachedCameraDistance = .5f;
        [SerializeField] private float attachedCameraOrbitSpeed = 90f;

        private Vector3 focusPoint, previousFocusPoint, camPosition, prevCamPosition = default;
        [SerializeField, Min(0f)] private float focusRadius = 5f;
        [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;
        [SerializeField] private float attachedCameraSpeed = .7f;

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


        private Vector3 prevPlayerPos;

        private void PositionCameraBehindPlayer()
        {
            bool notMoved = 
                !previousFocusPoint.Equals(default) && 
                !focusPoint.Equals(default) &&
                (previousFocusPoint - focusPoint).sqrMagnitude < 0.00001f;
            
            bool tooClose = !camPosition.Equals(default) &&
                Vector3.Distance(camPosition, focusPoint) <
                attachedCameraMinDistance;
            
            bool movedAway = !camPosition.Equals(default) &&
                Vector3.Distance(camPosition, focusPoint) > 
                attachedCameraMaxDistance;
            
            if (notMoved || tooClose)
            {
                Orbit();
            }
            else if (movedAway)
            {
                Follow();
            }
            else if (camPosition.Equals(default))
            {
                Focus();
            }
            else
            {
                LookAt();
            }

            void Orbit()
            {
                float angle = TrailAngle();

                if (Mathf.Abs(angle) < 1f)
                    return;

                var step = attachedCameraOrbitSpeed * Time.unscaledDeltaTime;
                var delta = angle * step;

                var rotY = Quaternion.AngleAxis(delta, transform.up);
                var rotX = Quaternion.AngleAxis(cameraAngle, sceneCamera.transform.right);
                var pos = focusPoint - (rotY * rotX * transform.forward) * attachedCameraDistance;
                camPosition = Vector3.MoveTowards(
                        sceneCamera.transform.position, pos,
                        attachedCameraSpeed * Time.unscaledDeltaTime);

                var dir = focusPoint - camPosition;

                sceneCamera.transform.SetPositionAndRotation(
                    camPosition,
                    Quaternion.LookRotation(dir));
            }

            void Follow()
            {
                prevCamPosition = camPosition;

                var angle = TrailAngle();
                var step = attachedCameraOrbitSpeed * Time.unscaledDeltaTime;
                var delta = (Mathf.Abs(angle) > 2f) ? 
                    angle * step : 0f;

                var rotY = Quaternion.AngleAxis(delta, transform.up);
                var rotX = Quaternion.AngleAxis(cameraAngle, sceneCamera.transform.right);
                var pos = focusPoint - (rotY * rotX * transform.forward) * attachedCameraDistance;

                camPosition = Vector3.MoveTowards(
                        sceneCamera.transform.position, pos,
                        attachedCameraSpeed * Time.unscaledDeltaTime);

                var dir = focusPoint - camPosition;

                sceneCamera.transform.SetPositionAndRotation(
                    camPosition,
                    Quaternion.LookRotation(dir));
            }

            void LookAt()
            {
                sceneCamera.transform.LookAt(focusPoint);
            }

            void Focus()
            {
                prevCamPosition = camPosition;

                var rotX = Quaternion.AngleAxis(cameraAngle, sceneCamera.transform.right);
                var pos = focusPoint - rotX * transform.forward * attachedCameraDistance;

                camPosition = pos;

                var dir = focusPoint - camPosition;

                sceneCamera.transform.SetPositionAndRotation(
                    camPosition,
                    Quaternion.LookRotation(dir));
            }
            // utils
            float TrailAngle()
            {
                var toCamera = Vector3.ProjectOnPlane(
                    sceneCamera.transform.position - transform.position, transform.up);
                var angle = Vector3.SignedAngle(
                    toCamera, -transform.forward,
                    transform.up);
                if (Mathf.Abs(angle) > 90f)
                    return 90f * Mathf.Sign(angle);
                return angle;
            }

        }

        private void PositionCameraBehindPlayer1()
        {
            Vector3 TargetCamPos(float distance)
            {
                var right = distance > attachedCameraMaxDistance ? 0f :
                    distance < attachedCameraMinDistance ? 1f :
                    0f;

                return focusPoint +
                    (right - 1) * attachedCameraMinDistance * 1.03f * transform.forward +
                    right * attachedCameraMinDistance * 1.07f * transform.right +
                    Vector3.up * attachedCameraYOffset;
            }
            var distance = Vector3.Distance(
                sceneCamera.transform.position, transform.position);

            Vector3 targetCamPosition = TargetCamPos(distance);

            if (camPosition.Equals(default) ||
                sceneCamera.transform.position.y != 
                    Mathf.Clamp(sceneCamera.transform.position.y, 
                    transform.position.y - .2f,
                    transform.position.y + .2f))
            {
                camPosition = targetCamPosition;
                prevCamPosition = camPosition;
            }
            else
            {
                var speed = 
                    distance < attachedCameraMinDistance ?
                    attachedCameraSpeed * 2f : 
                    distance > attachedCameraMaxDistance ?
                    attachedCameraSpeed :
                    attachedCameraSpeed * 0.5f;
                if ((sceneCamera.transform.position - targetCamPosition).sqrMagnitude > .000001f)
                    camPosition = Vector3.MoveTowards(
                        sceneCamera.transform.position, targetCamPosition, speed *
                        Time.deltaTime);
                else 
                    camPosition = targetCamPosition;
            }

            var camDirection = focusPoint - camPosition;

            sceneCamera.transform.SetPositionAndRotation(
                camPosition, Quaternion.LookRotation(camDirection));
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
                PositionCameraTowardsCenter();
            else {
                UpdateFocusPoint();
                PositionCameraBehindPlayer();
            }
        }
        void UpdateFocusPoint()
        {
            previousFocusPoint = focusPoint;
            Vector3 targetPoint = transform.position + Vector3.up * .02f;
            focusPoint = targetPoint;
            return;
            if (focusRadius > 0f)
            {
                float distance = Vector3.Distance(targetPoint, focusPoint);
                float t = 1f;
                if (distance > 0.01f && focusCentering > 0f)
                {
                    t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
                }
                if (distance > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / distance);
                }
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            }
            else
            {
                focusPoint = targetPoint;
            }
        }

    }
}