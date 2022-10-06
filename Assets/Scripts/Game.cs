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
        [SerializeField] private GameObject[] zombieCollectablePrefabs;
        [SerializeField] private GameObject[] spiderCollectablePrefabs;
        [SerializeField] private GameObject[] chestCollectablePrefabs;

        [SerializeField] private Color playerMarkerColor = Color.green;
        [SerializeField] private Color zombieMarkerColor = Color.yellow;
        [SerializeField] private Color spiderMarkerColor = Color.magenta;

        [SerializeField] private int MaxZombieCount = 10;
        [SerializeField] private int MaxSpiderCount = 3;
        [SerializeField] private float enemySpawnInterval = 5.0f;

        [SerializeField] private HUDScreen hud;
        [SerializeField] private HUDMarkersView markers;
        [SerializeField] private HUDLeaderBoardView score;
        [SerializeField] private HUDBalance balance;

        private GameObject enemies;
        private GameObject collectables;
        private Building building;
        private Player player;

        private Zombie[] zombies = new Zombie[10];
        private Spider[] spiders = new Spider[10];
        private Chest[] chests = new Chest[10];
        private Collectable[] drops = new Collectable[20];

        private readonly string playerId = "Player";
        private readonly string zombiesId = "Zombies";

        private UnitInfo playerScoreInfo = 
            new("Player", Color.green, 0, Color.green, 0, 0, Color.green);
        private UnitInfo zombiesScoreInfo = 
            new("Zombies", Color.yellow, 0, Color.red, 0, 0, Color.yellow);

        public event EventHandler OnPlayerSpawned;
        public event EventHandler OnPlayerKilled;

        private IngameSoundEvents soundEvents;
        private PlayerBalanceService balances;
        private PlayerPerkService perks;
        private PlayerPreferencesService prefs;

        private void Awake()
        {
            soundEvents = GetComponent<IngameSoundEvents>();
            balances = GetComponent<PlayerBalanceService>();
            perks = GetComponent<PlayerPerkService>();
            prefs = GetComponent<PlayerPreferencesService>();
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
            player.Prefs = prefs;

            chests = building.GetComponentsInChildren<Chest>();
            foreach (var chest in chests)
                chest.OnChestOpened += Chest_OnChestOpened;

            score.AddPlayer(playerId);
            score.UpdatePlayer(playerId, playerScoreInfo, true);
            
            score.AddPlayer(zombiesId);
            score.UpdatePlayer(zombiesId, zombiesScoreInfo, false);

            foreach(var key in enemyKeys)
                StartCoroutine(StartSpawnEnemy(key));

            player.InitPerkedItems(); 
            
            StartCoroutine(PositionPlayer(player, building, true));

            InitHUD();
        }


        private void Chest_OnChestOpened(Chest obj)
        {
            soundEvents.ChestOpen();
            SpawnCollectables(obj.transform, chestCollectablePrefabs, Random.Range(7, 14));
        }

        private void Player_OnUnitBeforeKilled(MovableUnit obj)
        {
            OnPlayerKilled?.Invoke(obj, null);

            balances.EnemyScore++;
            zombiesScoreInfo.Score = balances.EnemyScore;
            score.UpdatePlayer(zombiesId, zombiesScoreInfo, false);
            balance.SetCoinX(balances.CurrentCoinX);
        }

        private void Player_OnUnitKilled(MovableUnit obj) =>
            StartCoroutine(PositionPlayer((Player)obj, building));

        private void Player_OnActiveWeaponAttack(WeaponType arg1, int arg2) =>
            balance.SetAmmo(arg2);

        private void Player_OnWeaponSelected(WeaponType obj) =>
            SwitchHUDWeapon(obj);

        private void Zombie_OnUnitBeforeKilled(MovableUnit obj)
        {
            soundEvents.ZombieKilled();

            SpawnCollectables(obj.transform, zombieCollectablePrefabs, Mathf.FloorToInt(obj.SizeScale * 5));
            balances.PlayerScore++;
            playerScoreInfo.Score = balances.PlayerScore;
            score.UpdatePlayer(playerId, playerScoreInfo, true);
            balance.SetCoinX(balances.CurrentCoinX);
        }
        private void Zombie_OnUnitKilled(MovableUnit obj) =>
            StartCoroutine(PositionZombie((Zombie)obj, building));

        private void Spider_OnUnitBeforeKilled(MovableUnit obj)
        {
            soundEvents.ZombieKilled();

            SpawnCollectables(obj.transform, spiderCollectablePrefabs, Mathf.FloorToInt(obj.SizeScale * 20));
            balances.PlayerScore += 10;
            playerScoreInfo.Score = balances.PlayerScore;
            score.UpdatePlayer(playerId, playerScoreInfo, true);
            balance.SetCoinX(balances.CurrentCoinX);
        }
        private void Spider_OnUnitKilled(MovableUnit obj) =>
            StartCoroutine(PositionSpider((Spider)obj, building));


        private void Balances_OnBalanceChanged(CollectableType arg1, int arg2) =>
            UpdateHUDBalances(arg1, arg2);

        private void Perks_OnPerkChanged(PerkType arg1, int arg2)
        {
            // update HUD button (weapon icon)
            // update markers maybe (larger shield, shield color, etc.)
            player.UpdatePerk(arg1, arg2);
            UpdateHUDPerk(arg1, arg2);
        }

        private Zombie GetZombie()
        {
            var idx = Array.IndexOf(enemyKeys, EnemyType.Zombie);
            var prefab = enemyPrefabs[idx];

            var zombie = Instantiate(prefab).GetComponent<Zombie>();
            zombie.transform.SetParent(enemies.transform);
            zombie.OnUnitBeforeKilled += Zombie_OnUnitBeforeKilled;
            zombie.OnUnitKilled += Zombie_OnUnitKilled;
            zombie.SoundEvents = soundEvents;
            zombie.SizeScale = Random.Range(.5f, 2.0f);

            zombie.gameObject.SetActive(false);
            
            return zombie;
        }
        private Spider GetSpider()
        {
            var idx = Array.IndexOf(enemyKeys, EnemyType.Spider);
            var prefab = enemyPrefabs[idx];

            var spider = Instantiate(prefab).GetComponent<Spider>();
            spider.transform.SetParent(enemies.transform);
            spider.OnUnitBeforeKilled += Spider_OnUnitBeforeKilled;
            spider.OnUnitKilled += Spider_OnUnitKilled;
            spider.SoundEvents = soundEvents;
            spider.SizeScale = Random.Range(.5f, 2.0f);

            spider.gameObject.SetActive(false);

            return spider;
        }


        private IEnumerator StartSpawnEnemy(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Zombie:
                    {
                        Array.Resize(ref zombies, MaxZombieCount);
                        for (int i = 0; i < MaxZombieCount; i++)
                        {
                            zombies[i] = GetZombie();
                            yield return null;
                        }

                        for (int i = 0; i < MaxZombieCount; i++)
                        {
                            yield return new WaitForSeconds(enemySpawnInterval);

                            StartCoroutine(PositionZombie(zombies[i], building));
                        }
                    }
                    break;
                case EnemyType.Spider:
                    {
                        Array.Resize(ref spiders, MaxSpiderCount);
                        for (int i = 0; i < MaxSpiderCount; i++)
                        {
                            spiders[i] = GetSpider();
                            yield return null;
                        }

                        for (int i = 0; i < MaxSpiderCount; i++)
                        {
                            yield return new WaitForSeconds(enemySpawnInterval);

                            StartCoroutine(PositionSpider(spiders[i], building));
                        }

                    }
                    break;
                default:
                    break;
            }
        }

        private void SpawnCollectables(Transform location, GameObject[] source, int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                var prefab = source[Random.Range(0, source.Length)];
                Instantiate(prefab, location.position + Vector3.up * .05f + .01f * i * location.forward, Quaternion.identity, collectables.transform);
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

            player.ToggleInput(true);
        }

        private IEnumerator PositionSpider(Spider spider, Building building)
        {
            if (spider.isActiveAndEnabled)
            {
                while (!spider.FadeOut())
                    yield return null;
            }

            var filtered = building.SpiderSpawnPoints.Where(x => x.gameObject.activeSelf).ToArray();
            var spIndex = UnityEngine.Random.Range(0, filtered.Length);
            var sp = filtered[spIndex].transform;
            spider.Building = building;

            spider.gameObject.SetActive(true);
            spider.OnRespawned(sp.position, sp.rotation);

            if (spider.TryGetComponent<Visibility>(out var unit))
            {
                unit.MarkerColor = spiderMarkerColor;
                markers.AddEnemy(unit, EnemyType.Spider);
            }

            spider.FadeIn();
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