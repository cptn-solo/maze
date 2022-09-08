using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Transform[] PlayerSpawnPoints => playerSpawnPoints;
    
    private Transform[] playerSpawnPoints;

    private void Awake()
    {
        playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint>()
            .Select(x => x.transform).ToArray();
    }

}
