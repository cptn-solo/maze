using UnityEngine;

public class PlayerSpawnPoint : SpawnPoint
{
    private Building building;

    #if UNITY_EDITOR

    private void Start()
    {
        building = GetComponentInParent<Building>();
    }

    private void OnDrawGizmos()
    {
        if (building == null)
            return;

        var old = Gizmos.color;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(building.transform.position + Vector3.up * 10.0f, building.transform.position - Vector3.up * 10.0f);
        Gizmos.DrawRay(transform.position, (building.transform.position + Vector3.up * transform.position.y - transform.position) * 10.0f);
        Gizmos.color = old;
    }

    #endif
}
