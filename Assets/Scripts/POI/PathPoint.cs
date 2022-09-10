using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField] private Color color = Color.yellow;

    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        var old = Gizmos.color;
        
        Gizmos.color = color;
        
        Gizmos.DrawRay(transform.position, Vector3.down * 10.0f);

        Gizmos.DrawLine(transform.position - Vector3.forward * 10.0f, transform.position + Vector3.forward * 10.0f);
        Gizmos.DrawLine(transform.position - Vector3.right * 10.0f, transform.position + Vector3.right * 10.0f);
        
        Gizmos.color = old;

    }

    #endif

}
