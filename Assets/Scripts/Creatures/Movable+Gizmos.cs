using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class MovableUnit
    {

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var old = Gizmos.color;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, localForward * 10.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, localRight * 10.0f);

            Gizmos.color = old;

        }
#endif
    }
}