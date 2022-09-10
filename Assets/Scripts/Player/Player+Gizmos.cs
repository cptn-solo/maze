using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Player
    {

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var old = Gizmos.color;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, localForward * 10.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, localRight * 10.0f);

            if (closest1 != default)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(closest1 + Vector3.up * transform.position.y, .1f);
            }
            if (closest2 != default)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(closest2 + Vector3.up * transform.position.y, .1f);
            }

            if (targetPosition != default)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(targetPosition + Vector3.up * transform.position.y, .02f);
            }
            if (sideSwitchPivot != default)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(sideSwitchPivot + Vector3.up * transform.position.y, .02f);
            }

            Gizmos.color = old;

        }
#endif
    }
}