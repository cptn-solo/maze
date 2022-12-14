using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class WallTeleportRouter : MonoBehaviour
    {
        private TeleportPoint[] wallTp;
        private Vector3[] points;
        private Dictionary<Vector3, TeleportPoint> portals;
        [SerializeField] private float transferSpeed = 2.0f;

        private void Awake()
        {
            wallTp = GetComponentsInChildren<TeleportPoint>();
            portals = new Dictionary<Vector3, TeleportPoint>(wallTp.Length);

            foreach (var item in wallTp)
                portals.Add(item.transform.position, item);

            points = portals.Keys.ToArray();
        }

        private void OnDisable()
        {
            foreach (var tp in wallTp)
                tp.OnEnterPortal -= Tp_OnEnterPortal;
        }
        private void OnEnable()
        {
            foreach (var tp in wallTp)
                tp.OnEnterPortal += Tp_OnEnterPortal;
        }

        private void Tp_OnEnterPortal(Vector3 point, Vector3 dir, MovableUnit passenger)
        {
            var filtered = points.Where(x => x != point && x.y != Mathf.Clamp(x.y, point.y - .01f, point.y + .01f));
            filtered = filtered.Where(x =>
            {
                var exitGate = portals[x].ExitGate(dir);
                return exitGate != null;
            });

            var ordered = filtered.OrderBy(x => (x - point).sqrMagnitude).ToArray();

            var closest = ordered.Length > 2 ? ordered.Take(2).ToArray() :  new[] { ordered[0] };
            var idx = Random.Range(0, closest.Length);

            var dest = closest[idx];

            StartCoroutine(TransferTo(passenger, dest + dir * 0.2f));
        }

        private IEnumerator TransferTo(MovableUnit passenger, Vector3 dest)
        {
            while (!passenger.FadeOut())
                yield return null;

            bool transferFinished = false;
            var startPos = passenger.transform.position;
            var distance = Vector3.Distance(startPos, dest);
            var dir = (dest - passenger.transform.position).normalized;
            var step = distance * transferSpeed * Time.deltaTime * dir;

            while (!transferFinished)
            {
                passenger.transform.position += step;
                distance -= step.magnitude;
                transferFinished = distance <= 0.0001;
                yield return null;
            }

            passenger.transform.position = dest;

            passenger.FadeIn();
        }

    }
}