using System.Collections;
using System.Linq;
using UnityEngine;

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

        [SerializeField] private Camera sceneCamera;

        [SerializeField] private float cameraAngle = -35.0f;
        [SerializeField] private float cameraDistance = 2.5f;
        [SerializeField] private float camSpeed = 4.0f;

        private float camDistanceFactor = 1.0f;

        private Building building;
        private Player player;
        private bool listenForScreenOrientation;
        private bool listeningForScreenOrientation;

        private Zombie[] zombies = new Zombie[1];

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

        private IEnumerator StartSpawnEnemy(int i)
        {
            while (true)
            {
                Zombie zombie = zombies.Length >= MaxZombieCount ?
                    zombies[Random.Range(0, zombies.Length)] :
                    Instantiate(enemyPrefabs[i]).GetComponent<Zombie>();

                PositionZombie(zombie, building);

                if (zombies.Contains(zombie))
                    zombies.Append(zombie);

                yield return new WaitForSeconds(5.0f);
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
        }

        private void PositionZombie(Zombie zombie, Building building)
        {
            var spIndex = UnityEngine.Random.Range(0, building.ZombieSpawnPoints.Length);
            var sp = building.ZombieSpawnPoints[spIndex].transform;
            zombie.Building = building;

            zombie.OnRespawned(sp.position, sp.rotation);

        }

        private void LateUpdate()
        {
            PositionCamera(player, building);

            if ((player.transform.position - Vector3.zero).sqrMagnitude > 100.0f)
                PositionPlayer(player, building);
        }
    }
}