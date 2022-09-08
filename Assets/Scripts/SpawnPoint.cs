using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private float radius = .5f;
    [SerializeField] private Color color = Color.blue;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        var old = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = old;
    }

#endif

}
