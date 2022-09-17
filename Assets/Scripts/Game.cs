using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private EnemyType[] enemyKeys;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private GameObject[] buildingPrefabs;
        [SerializeField] private GameObject[] collectablePrefabs;
        
        [SerializeField] private int MaxZombieCount = 10;
        [SerializeField] private float enemySpawnInterval = 5.0f;

        [SerializeField] private Camera sceneCamera;

        [SerializeField] private float cameraAngle = -35.0f;
        [SerializeField] private float cameraDistance = 2.5f;
        [SerializeField] private float camSpeed = 4.0f;

        [SerializeField] private HUDMarkersView markers;

        private float camDistanceFactor = 1.0f;

        private Building building;
        private Player player;
        private bool listenForScreenOrientation;
        private bool listeningForScreenOrientation;

        private Zombie[] zombies = new Zombie[10];

        // Start is called before the first frame update
        void Start()
        {
            building = Instantiate(buildingPrefabs[0]).GetComponent<Building>();
            player = Instantiate(playerPrefab).GetComponent<Player>();

            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                StartCoroutine(StartSpawnEnemy(i));
            }

            PositionPlayer(player, building);
            PositionCamera(player, building);            
        }

        private IEnumerator StartSpawnEnemy(int enemyPrefabIdx)
        {

            Array.Resize(ref zombies, MaxZombieCount);
            var prefab = enemyPrefabs[enemyPrefabIdx];
            for (int i = 0; i < MaxZombieCount; i++)
            {
                var zombie = Instantiate(prefab).GetComponent<Zombie>();
                zombie.gameObject.SetActive(false);
                zombies[i] = zombie;
            }

            while (true)
            {
                var zombie = zombies[Random.Range(0, zombies.Length)];

                PositionZombie(zombie, building);

                yield return new WaitForSeconds(enemySpawnInterval);
            }
        }

        private void OnEnable()
        {
            listenForScreenOrientation = true;
            if (!listeningForScreenOrientation)
                StartCoroutine(ScreenOrientationMonitor());
        }

        private void OnDisable()
        {
            listenForScreenOrientation = false;
        }

        private IEnumerator ScreenOrientationMonitor()
        {
            listeningForScreenOrientation = true;
            while (listenForScreenOrientation)
            {
                camDistanceFactor = ((float) Screen.height) / Screen.width;
                yield return new WaitForSecondsRealtime(1.0f);
            }
        }

        private void PositionCamera(Player player, Building building)
        {
            var buildingFloorY = Vector3.zero + Vector3.up * player.transform.position.y;
            var buildingToPlayer = Vector3.Distance(player.transform.position, buildingFloorY);

            var yOffset = Mathf.Tan(cameraAngle * Mathf.Deg2Rad) * buildingToPlayer;

            var cameraRay = new Ray(
                buildingFloorY,
                (player.transform.position + player.transform.up * yOffset - buildingFloorY).normalized);
            
            var camCurrent = sceneCamera.transform.position;

            var camPosition = player.transform.position + cameraRay.direction * cameraDistance * camDistanceFactor;
            var camDirection = player.transform.position - camPosition;

            var camStep = (camPosition - camCurrent) * camSpeed * Time.deltaTime;

            sceneCamera.transform.SetPositionAndRotation(
                camCurrent + camStep, Quaternion.LookRotation(camDirection));
        }

        private void PositionPlayer(Player player, Building building)
        {
            var spIndex = UnityEngine.Random.Range(0, building.PlayerSpawnPoints.Length);
            var sp = building.PlayerSpawnPoints[spIndex].transform;
            player.Building = building;

            player.OnRespawned(sp.position, sp.rotation);

            if (player.TryGetComponent<Visibility>(out var unit))
                markers.AddUnit(unit);
        }

        private void PositionZombie(Zombie zombie, Building building)
        {
            zombie.gameObject.SetActive(false);

            var spIndex = UnityEngine.Random.Range(0, building.ZombieSpawnPoints.Length);
            var sp = building.ZombieSpawnPoints[spIndex].transform;
            zombie.Building = building;
            
            zombie.gameObject.SetActive(true);
            zombie.OnRespawned(sp.position, sp.rotation);

            if (zombie.TryGetComponent<Visibility>(out var unit)) 
                markers.AddEnemy(unit, EnemyType.Zombie);
        }

        private void LateUpdate()
        {
            PositionCamera(player, building);

            if ((player.transform.position - Vector3.zero).sqrMagnitude > 100.0f)
                PositionPlayer(player, building);
        }
    }
}