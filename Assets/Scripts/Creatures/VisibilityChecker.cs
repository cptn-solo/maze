using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class VisibilityChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private bool isPlayer;
        [SerializeField] private float visibilityDistance = .3f;

        private readonly Collider[] buff = new Collider[1]; 

        public event EventHandler<bool> OnVisibilityChanged;
        
        public Transform PlayerTransform { get; set; }

        private Transform cam;
        private bool visible;

        private void Awake()
        {
            cam = Camera.main.transform;
        }

        private void OnEnable()
        {
            visible = false;
            OnVisibilityChanged?.Invoke(this, visible);

            _ = StartCoroutine(nameof(CheckForVisible));
        }

        private void OnDisable()
        {
            visible = false;
            OnVisibilityChanged?.Invoke(this, visible);

            StopCoroutine(nameof(CheckForVisible));
        }

        private IEnumerator CheckForVisible()
        {
            while (true)
            {
                if (!Time.inFixedTimeStep)
                    yield return new WaitForFixedUpdate();

                var direction = transform.position + .1f * transform.localScale.y * Vector3.up - cam.position;
                Ray ray = default;
                ray.direction = direction;
                ray.origin = cam.position;
                var distance = direction.magnitude;
                
                var playerNearby = isPlayer || Physics.OverlapSphereNonAlloc(transform.position, visibilityDistance, buff, playerMask) > 0;

                visible = playerNearby && Physics.Raycast(ray, out var hitinfo, distance, layerMask) &&
                    hitinfo.collider.gameObject == gameObject;

                OnVisibilityChanged?.Invoke(this, visible);

                yield return new WaitForSeconds(0.5f);
            }
        }

    }
}