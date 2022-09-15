using Assets.Scripts.POI;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Building : MonoBehaviour
    {
        public Transform[] PlayerSpawnPoints => playerSpawnPoints;
        public Transform[] ZombieSpawnPoints => zombieSpawnPoints;

        private Transform[] playerSpawnPoints;
        private Transform[] zombieSpawnPoints;

        private void Awake()
        {
            playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint>()
                .Select(x => x.transform).ToArray();

            zombieSpawnPoints = GetComponentsInChildren<ZombieSpawnPoint>()
                .Select(x => x.transform).ToArray();
        }

    }
}