using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public partial class Game : MonoBehaviour, IIngameMusicEvents
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private EnemyType[] enemyKeys;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private GameObject[] buildingPrefabs;
        [SerializeField] private GameObject[] collectablePrefabs;

        [SerializeField] private Color playerMarkerColor = Color.green;
        [SerializeField] private Color zombieMarkerColor = Color.yellow;

        [SerializeField] private int MaxZombieCount = 10;
        [SerializeField] private float enemySpawnInterval = 5.0f;

        [SerializeField] private HUDMarkersView markers;
        [SerializeField] private HUDLeaderBoardView score;
        [SerializeField] private HUDBalance balance;

        private GameObject enemies;
        private GameObject collectables;
        private Building building;
        private Player player;

        private Zombie[] zombies = new Zombie[10];
        private Chest[] chests = new Chest[10];
        private Collectable[] drops = new Collectable[20];

        private readonly string playerId = "Player";
        private readonly string zombiesId = "Zombies";

        private UnitInfo playerScoreInfo = new("Player", Color.green, 0, Color.green, 0, 0, Color.green);
        private UnitInfo zombiesScoreInfo = new("Zombies", Color.yellow, 0, Color.red, 0, 0, Color.yellow);

        public event EventHandler OnPlayerSpawned;
        public event EventHandler OnPlayerKilled;

        private IngameSoundEvents soundEvents;
        private PlayerBalanceService balances;
        private PlayerPerkService perks;

        private void Awake()
        {
            soundEvents = GetComponent<IngameSoundEvents>();
            balances = GetComponent<PlayerBalanceService>();
            perks = GetComponent<PlayerPerkService>();
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
            player.Perks = perks;

            chests = building.GetComponentsInChildren<Chest>();
            foreach (var chest in chests)
                chest.OnChestOpened += Chest_OnChestOpened;

            score.AddPlayer(playerId);
            score.UpdatePlayer(playerId, playerScoreInfo, true);
            
            score.AddPlayer(zombiesId);
            score.UpdatePlayer(zombiesId, zombiesScoreInfo, false);

            for (int i = 0; i < enemyPrefabs.Length; i++)
                StartCoroutine(StartSpawnEnemy(i));

            player.InitPerkedItems(); 
            
            StartCoroutine(PositionPlayer(player, building, true));

            InitHUD();            
        }

        private void Chest_OnChestOpened(Chest obj)
        {
            soundEvents.ChestOpen();
            var prefab = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
            Instantiate(prefab, obj.transform.position + Vector3.up * .07f - obj.transform.forward * .07f, Quaternion.identity,
                collectables.transform);
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
        private void Player_OnWeaponSelected(WeaponType obj)
        {
            SwitchHUDWeapon(obj);
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

        private void Balances_OnBalanceChanged(CollectableType arg1, int arg2)
        {
            UpdateHUDBalances(arg1, arg2);
        }

        private void Perks_OnPerkChanged(PerkType arg1, int arg2)
        {
            // update HUD button (weapon icon)
            // update markers maybe (larger shield, shield color, etc.)
            player.UpdatePerk(arg1, arg2);
            UpdateHUDPerk(arg1, arg2);
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

        private void SpawnCollectables(MovableUnit obj)
        {
            var cnt = Mathf.FloorToInt(obj.SizeScale * 5);
            for (int i = 0; i < cnt; i++)
            {
                var prefab = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
                Instantiate(prefab, obj.transform.position + Vector3.up * .05f + .01f * i * obj.transform.forward, Quaternion.identity, collectables.transform);
            }
        }

        private void OnEnable()
        {
            balances.OnBalanceChanged += Balances_OnBalanceChanged;
            perks.OnPerkChanged += Perks_OnPerkChanged;
        }

        private void OnDisable()
        {
            balances.OnBalanceChanged -= Balances_OnBalanceChanged;
            perks.OnPerkChanged -= Perks_OnPerkChanged;
        }

        private IEnumerator PositionPlayer(Player player, Building building, bool startingPoint = false)
        {
            while (!player.FadeOut())
                yield return null;

            var filtered = building.PlayerSpawnPoints.Where(x => x.gameObject.activeSelf && 
                (!startingPoint || x.StartingPoint) ).ToArray();

            var spIndex = UnityEngine.Random.Range(0, filtered.Length);
            var sp = filtered[spIndex].transform;
            player.Building = building;

            player.OnRespawned(sp.position, sp.rotation);

            if (player.TryGetComponent<Visibility>(out var unit))
            {
                unit.MarkerColor = playerMarkerColor;
                markers.AddUnit(unit);
            }

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
            {
                unit.MarkerColor = zombieMarkerColor;
                markers.AddEnemy(unit, EnemyType.Zombie);
            }

            zombie.FadeIn();
        }        
    }
}