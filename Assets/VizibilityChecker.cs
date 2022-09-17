using System;
using System.Collections;
using UnityEngine;

public class VizibilityChecker : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    public event EventHandler<bool> OnVisibilityChanged;
    
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

            var direction = ((transform.position + Vector3.up * .1f) - cam.position);
            Ray ray = default;
            ray.direction = direction;
            ray.origin = cam.position;

            visible = (Physics.Raycast(ray, out var hitinfo, direction.magnitude, layerMask) &&
                hitinfo.collider.gameObject == this.gameObject);

            OnVisibilityChanged?.Invoke(this, visible);

            yield return new WaitForSeconds(0.2f);
        }
    }

}
