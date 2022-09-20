using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour, IIngameMusicEvents
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
        [SerializeField] private HUDLeaderBoardView score;

        private float camDistanceFactor = 1.0f;

        private Building building;
        private Player player;
        private bool listenForScreenOrientation;
        private bool listeningForScreenOrientation;

        private Zombie[] zombies = new Zombie[10];

        private readonly string playerId = "Player";
        private readonly string zombiesId = "Zombies";

        private UnitInfo playerScoreInfo = new("Player", Color.green, 0, Color.green);
        private UnitInfo zombiesScoreInfo = new("Zombies", Color.red, 0, Color.red);

        public event EventHandler OnPlayerSpawned;
        public event EventHandler OnPlayerKilled;

        private IngameSoundEvents soundEvents;

        private void Awake()
        {
            soundEvents = GetComponent<IngameSoundEvents>();
        }

        void Start()
        {
            building = Instantiate(buildingPrefabs[0]).GetComponent<Building>();
            player = Instantiate(playerPrefab).GetComponent<Player>();

            player.OnUnitBeforeKilled += Player_OnUnitBeforeKilled;
            player.OnUnitKilled += Player_OnUnitKilled;
            player.SoundEvents = soundEvents;

            score.AddPlayer(playerId);
            score.UpdatePlayer(playerId, playerScoreInfo, true);
            
            score.AddPlayer(zombiesId);
            score.UpdatePlayer(zombiesId, zombiesScoreInfo, false);

            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                StartCoroutine(StartSpawnEnemy(i));
            }

            StartCoroutine(PositionPlayer(player, building));
        }

        private void Player_OnUnitBeforeKilled(MovableUnit obj)
        {
            OnPlayerKilled?.Invoke(obj, null);
            
            zombiesScoreInfo.Score++;
            score.UpdatePlayer(zombiesId, zombiesScoreInfo, false);
        }

        private void Player_OnUnitKilled(MovableUnit obj)
        {
            StartCoroutine(PositionPlayer((Player)obj, building));
        }

        private IEnumerator StartSpawnEnemy(int enemyPrefabIdx)
        {

            Array.Resize(ref zombies, MaxZombieCount);
            var prefab = enemyPrefabs[enemyPrefabIdx];
            for (int i = 0; i < MaxZombieCount; i++)
            {
                var zombie = Instantiate(prefab).GetComponent<Zombie>();
                zombie.OnUnitBeforeKilled += Zombie_OnUnitBeforeKilled;
                zombie.OnUnitKilled += Zombie_OnUnitKilled;
                zombie.SoundEvents = soundEvents;
                
                zombie.gameObject.SetActive(false);

                zombies[i] = zombie;
                yield return null;
            }

            for (int i = 0; i < MaxZombieCount; i++)
            {
                yield return new WaitForSeconds(enemySpawnInterval);

                var zombie = zombies[i];

                StartCoroutine(PositionZombie(zombie, building));
            }
        }

        private void Zombie_OnUnitBeforeKilled(MovableUnit obj)
        {
            soundEvents.ZombieKilled();

            playerScoreInfo.Score++;
            score.UpdatePlayer(playerId, playerScoreInfo, true);
        }

        private void Zombie_OnUnitKilled(MovableUnit obj)
        {
            StartCoroutine(PositionZombie((Zombie)obj, building));
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

        private IEnumerator PositionPlayer(Player player, Building building)
        {
            while (!player.FadeOut())
                yield return null;

            var filtered = building.PlayerSpawnPoints.Where(x => x.gameObject.activeSelf).ToArray();

            var spIndex = UnityEngine.Random.Range(0, filtered.Length);
            var sp = filtered[spIndex].transform;
            player.Building = building;

            player.OnRespawned(sp.position, sp.rotation);

            if (player.TryGetComponent<Visibility>(out var unit))
                markers.AddUnit(unit);

            player.FadeIn();

            OnPlayerSpawned?.Invoke(player, null);
        }

        private IEnumerator PositionZombie(Zombie zombie, Building building)
        {
            if (zombie.isActiveAndEnabled)
            {
                while (!zombie.FadeOut())
                    yield return null;
            }

            var filtered = building.ZombieSpawnPoints.Where(x => x.gameObject.activeSelf).ToArray();
            var spIndex = UnityEngine.Random.Range(0, filtered.Length);
            var sp = filtered[spIndex].transform;
            zombie.Building = building;
            
            zombie.gameObject.SetActive(true);
            zombie.OnRespawned(sp.position, sp.rotation);

            if (zombie.TryGetComponent<Visibility>(out var unit)) 
                markers.AddEnemy(unit, EnemyType.Zombie);

            zombie.FadeIn();
        }

        private void LateUpdate()
        {
            PositionCamera(player, building);

            if ((player.transform.position - Vector3.zero).sqrMagnitude > 100.0f)
                StartCoroutine(PositionPlayer(player, building));
        }
    }
}