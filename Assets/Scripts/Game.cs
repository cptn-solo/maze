using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
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
        [SerializeField] private HUDBalance balance;

        private float camDistanceFactor = 1.0f;
        private GameObject enemies;
        private GameObject collectables;
        private Building building;
        private Player player;
        private bool listenForScreenOrientation;
        private bool listeningForScreenOrientation;

        private Zombie[] zombies = new Zombie[10];
        private Chest[] chests = new Chest[10];
        private Collectable[] drops = new Collectable[20];

        private readonly string playerId = "Player";
        private readonly string zombiesId = "Zombies";

        private UnitInfo playerScoreInfo = new("Player", Color.green, 0, Color.green, 0, 0);
        private UnitInfo zombiesScoreInfo = new("Zombies", Color.red, 0, Color.red, 0, 0);

        public event EventHandler OnPlayerSpawned;
        public event EventHandler OnPlayerKilled;

        public event Action<WallmartItem, string> OnWallmartApproached;
        public event Action<WallmartItem, string> OnWallmartBuyButton;
        public event Action OnWallmartLeft;

        private IngameSoundEvents soundEvents;
        private PlayerBalanceService balances;

        private void Awake()
        {
            soundEvents = GetComponent<IngameSoundEvents>();
            balances = GetComponent<PlayerBalanceService>();
        }

        void Start()
        {
            enemies = new GameObject("Enemies");
            collectables = new GameObject("Collectables");

            building = Instantiate(buildingPrefabs[0]).GetComponent<Building>();
            player = Instantiate(playerPrefab).GetComponent<Player>();

            player.OnUnitBeforeKilled += Player_OnUnitBeforeKilled;
            player.OnUnitKilled += Player_OnUnitKilled;
            player.OnWeaponSelected += Player_OnWeaponSelected;
            player.OnActiveWeaponAttack += Player_OnActiveWeaponAttack;
            player.OnWallmartApproached += Player_OnWallmartApproached;
            player.OnWallmartLeft += Player_OnWallmartLeft;
            
            player.SoundEvents = soundEvents;
            player.Balances = balances;

            chests = building.GetComponentsInChildren<Chest>();
            foreach (var chest in chests)
                chest.OnChestOpened += Chest_OnChestOpened;

            score.AddPlayer(playerId);
            score.UpdatePlayer(playerId, playerScoreInfo, true);
            
            score.AddPlayer(zombiesId);
            score.UpdatePlayer(zombiesId, zombiesScoreInfo, false);

            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                StartCoroutine(StartSpawnEnemy(i));
            }

            StartCoroutine(PositionPlayer(player, building));

            balance.SetBalance(balances.CurrentBalance(CollectableType.Coin));
            balance.CurrentWeapon = WeaponType.Shuriken;
            balance.SetStowedAmmo(balances.CurrentBalance(CollectableType.Minigun));
            balance.SetItemAmmo(CollectableType.Bomb, balances.CurrentBalance(CollectableType.Bomb));
            balance.SetItemAmmo(CollectableType.Landmine, balances.CurrentBalance(CollectableType.Landmine));

        }

        private void Player_OnWallmartLeft()
        {
            OnWallmartLeft?.Invoke();
        }

        private void Player_OnWallmartApproached(WallmartItem arg1, string arg2)
        {
            OnWallmartApproached?.Invoke(arg1, arg2);
        }

        private void Chest_OnChestOpened(Chest obj)
        {
            soundEvents.ChestOpen();
            var prefab = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
            Instantiate(prefab, obj.transform.position + Vector3.up * .07f - obj.transform.forward * .07f, Quaternion.identity,
                collectables.transform);
        }

        private void SpawnCollectables(MovableUnit obj)
        {
            var cnt = Mathf.FloorToInt(obj.SizeScale * 5);
            for (int i = 0; i < cnt; i++)
            {
                var prefab = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
                Instantiate(prefab, obj.transform.position + Vector3.up * .05f + .01f * i * obj.transform.forward, Quaternion.identity, collectables.transform);
            }
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

        private void Player_OnActiveWeaponAttack(WeaponType arg1, int arg2)
        {
            balance.SetAmmo(arg2);
        }

        private void Balances_OnBalanceChanged(CollectableType arg1, int arg2)
        {
            if (arg1 == CollectableType.Coin)
                balance.SetBalance(arg2);
            else if (PlayerBalanceService.CollectableForWeapon(balance.CurrentWeapon) == arg1)
                balance.SetAmmo(arg2);
            else if (PlayerBalanceService.CollectableForWeapon(balance.StowedWeapon) == arg1)
                balance.SetStowedAmmo(arg2);
            else
                balance.SetItemAmmo(arg1, arg2);
        }

        private void Player_OnWeaponSelected(WeaponType obj)
        {
            var stowedWeapon = balance.CurrentWeapon;
            balance.CurrentWeapon = obj;
            var ammoCollectable = PlayerBalanceService.CollectableForWeapon(balance.CurrentWeapon);
            if (ammoCollectable != CollectableType.NA)
                balance.SetAmmo(balances.CurrentBalance(ammoCollectable));

            var stowedAmmoCollectable = PlayerBalanceService.CollectableForWeapon(stowedWeapon);
            var stowedCount = stowedAmmoCollectable != CollectableType.NA ?
                balances.CurrentBalance(stowedAmmoCollectable) :
                -1;
            balance.SetStowedAmmo(stowedCount);

        }

        private IEnumerator StartSpawnEnemy(int enemyPrefabIdx)
        {

            Array.Resize(ref zombies, MaxZombieCount);
            var prefab = enemyPrefabs[enemyPrefabIdx];
            for (int i = 0; i < MaxZombieCount; i++)
            {
                var zombie = Instantiate(prefab).GetComponent<Zombie>();
                zombie.transform.SetParent(enemies.transform);
                zombie.OnUnitBeforeKilled += Zombie_OnUnitBeforeKilled;
                zombie.OnUnitKilled += Zombie_OnUnitKilled;
                zombie.SoundEvents = soundEvents;
                zombie.SizeScale = Random.Range(.5f, 2.0f);

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

            SpawnCollectables(obj);
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

            balances.OnBalanceChanged += Balances_OnBalanceChanged;
        }

        private void OnDisable()
        {
            listenForScreenOrientation = false;
            balances.OnBalanceChanged -= Balances_OnBalanceChanged;
        }

        private IEnumerator ScreenOrientationMonitor()
        {
            listeningForScreenOrientation = true;
            while (listenForScreenOrientation)
            {
                camDistanceFactor = ((float) Screen.height) * .7f / Screen.width;
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

            var camPosition = player.transform.position + camDistanceFactor * cameraDistance * cameraRay.direction;
            var camDirection = player.transform.position - camPosition;

            var camStep = camSpeed * Time.deltaTime * (camPosition - camCurrent);

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

        internal bool BuyItem(WallmartItem item, string playerId)
        {
            return true;
        }
    }
}