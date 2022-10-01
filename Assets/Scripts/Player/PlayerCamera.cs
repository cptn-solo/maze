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
        
        private bool listeningForScreenOrientation;
        private float camDistanceFactor = 1.0f;

        private Player player;
        private Camera sceneCamera;
        //private OrbitCamera orbitCamera;

        [SerializeField] private float attachedCameraYOffset = 0.15f;
        [SerializeField] private float attachedCameraZOffset = -0.15f;
        [SerializeField] private float attachedCameraFollowDelay = 1f;

        private Vector3 focusPoint, previousFocusPoint, camPosition, prevCamPosition = default;
        [SerializeField, Min(0f)] private float focusRadius = 5f;
        [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;

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
            if (prevCamPosition.Equals(default))
                camPosition = focusPoint + 
                    attachedCameraYOffset * Vector3.up +
                    attachedCameraZOffset * player.transform.forward;
            
            if (Vector3.Distance(camPosition, focusPoint) > attachedCameraYOffset + attachedCameraYOffset)
            {
                var targetCamPosition = focusPoint +
                    attachedCameraYOffset * Vector3.up +
                    attachedCameraZOffset * player.transform.forward;
                camPosition = Vector3.MoveTowards(camPosition, targetCamPosition, camSpeed * Time.unscaledDeltaTime);
            }

            var camDirection = focusPoint + (attachedCameraYOffset/2) * Vector3.up - camPosition;

            sceneCamera.transform.SetPositionAndRotation(
                camPosition, Quaternion.LookRotation(camDirection));

            prevCamPosition = camPosition;
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
            Vector3 targetPoint = transform.position;
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