using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField] private LayerMask avoidMask;
    [SerializeField] private LayerMask passMask;

    public event EventHandler OnDeadlock;

    private const float MaxScanDistance = .15f;
    private const float DeadlockDistance = .08f;
    private const float DeadlockSeconds = 5.0f;
    private const float ScanYOffset = .05f;

    private Vector3 overrideDir = default;
    private Vector3 prevPos = default;

    private bool drifting;

    internal Vector3 SuggestedDir(Vector3 desiredDir)
    {
        return overrideDir == default ? desiredDir : overrideDir;
    }

    private void OnEnable()
    {
        StartCoroutine(Scan());
        StartCoroutine(DeadlockDetection());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DeadlockDetection()
    {
        while (true)
        {
            prevPos = transform.position;
            
            yield return new WaitForSeconds(DeadlockSeconds);

            if (prevPos.x == Mathf.Clamp(prevPos.x, transform.position.x - DeadlockDistance, transform.position.x + DeadlockDistance) &&
                prevPos.z == Mathf.Clamp(prevPos.z, transform.position.z - DeadlockDistance, transform.position.z + DeadlockDistance))
                OnDeadlock?.Invoke(this, null);
        }

    }

    private IEnumerator Scan()
    {
        while (true)
        {
            if (!drifting &&
                !Physics.Raycast(transform.position + Vector3.up * ScanYOffset, transform.forward, MaxScanDistance, passMask) &&
                Physics.Raycast(transform.position + Vector3.up * ScanYOffset, transform.forward, MaxScanDistance, avoidMask))
            {
                var left45Dir = Quaternion.AngleAxis(-45f, transform.up) * transform.forward;
                var left45 = !Physics.Raycast(transform.position + Vector3.up * ScanYOffset, left45Dir, MaxScanDistance, avoidMask);

                var right45Dir = Quaternion.AngleAxis(45f, transform.up) * transform.forward;
                var right45 = !Physics.Raycast(transform.position + Vector3.up * ScanYOffset, right45Dir, MaxScanDistance, avoidMask);

                if (left45 && right45)
                    overrideDir = Random.Range(0, 2) == 0 ? left45Dir : right45Dir;
                else if (left45)
                    overrideDir = left45Dir;
                else if (right45)
                    overrideDir = right45Dir;
                else
                {
                    var ways = new []{
                        -transform.right,
                        transform.right,
                        -transform.forward,
                        Quaternion.AngleAxis(-45f, transform.up) * -transform.right,
                        Quaternion.AngleAxis(45f, transform.up) * transform.right
                    };
                    overrideDir = ways[Random.Range(0, 5)];
                }

                drifting = true;

                StartCoroutine(Drift());
            }

            yield return new WaitForSeconds(1.0f);
        }
        
    }
    private IEnumerator Drift()
    {
        yield return new WaitForSeconds(1.0f);

        drifting = false;
        overrideDir = default;
    }
}
