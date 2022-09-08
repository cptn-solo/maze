using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Transform[] PlayerSpawnPoints => playerSpawnPoints;
    
    private Transform[] playerSpawnPoints;
    public Transform[] PathPoints { get; private set; }

    private void Awake()
    {
        playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint>()
            .Select(x => x.transform).ToArray();

        PathPoints = GetComponentsInChildren<PathPoint>().Select(x => x.transform).ToArray();
    }

}
