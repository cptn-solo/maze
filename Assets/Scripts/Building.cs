using Assets.Scripts.POI;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Building : MonoBehaviour
    {
        public PlayerSpawnPoint[] PlayerSpawnPoints => 
            playerSpawnPoints.Select(x => x.GetComponent<PlayerSpawnPoint>()).ToArray();
        public Transform[] ZombieSpawnPoints => zombieSpawnPoints;
        public Transform[] SpiderSpawnPoints => spiderSpawnPoints;

        private Transform[] playerSpawnPoints;
        private Transform[] zombieSpawnPoints;
        private Transform[] spiderSpawnPoints;

        private void Awake()
        {
            playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint>()
                .Select(x => x.transform).ToArray();

            zombieSpawnPoints = GetComponentsInChildren<ZombieSpawnPoint>()
                .Select(x => x.transform).ToArray();

            spiderSpawnPoints = GetComponentsInChildren<SpiderSpawnPoint>()
                .Select(x => x.transform).ToArray();
        }
    }
}