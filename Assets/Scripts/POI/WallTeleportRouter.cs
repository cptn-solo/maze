using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        private void Tp_OnEnterPortal(Vector3 point, Vector3 dir, Player player)
        {
            var filtered = points.Where(x => x != point);
            var ordered = filtered.OrderBy(x => (x - point).sqrMagnitude).ToArray();

            var closest = ordered.Take(2).ToArray();
            var idx = Random.Range(0, 2);

            var dest = closest[idx];

            StartCoroutine(TransferPlayerTo(player, dest + dir * 0.2f));
        }

        private IEnumerator TransferPlayerTo(Player player, Vector3 dest)
        {
            while (!player.FadeOut())
                yield return null;

            bool transferFinished = false;
            var startPos = player.transform.position;
            var distance = Vector3.Distance(startPos, dest);
            var dir = (dest - player.transform.position).normalized;
            var step = distance * transferSpeed * Time.deltaTime * dir;

            while (!transferFinished)
            {
                player.transform.position += step;
                distance -= step.magnitude;
                transferFinished = distance <= 0.0001;
                yield return null;
            }

            player.transform.position = dest;

            player.FadeIn();
        }

    }
}