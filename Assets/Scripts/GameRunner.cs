using Assets.Scripts.UI;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameRunner: MonoBehaviour
    {
        [SerializeField] private bool pauseBetweenLevels;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject playerPrefab;

        private PlayerBalanceService balances;
        private PlayerPerkService perks;
        private PlayerPreferencesService prefs;

        public PlayerBalanceService Balances => balances;
        public PlayerPerkService Perks => perks;
        public PlayerPreferencesService Prefs => prefs;

        private Player player;

        public Player Player => player;

        private void Awake()
        {
            balances = GetComponent<PlayerBalanceService>();
            perks = GetComponent<PlayerPerkService>();
            prefs = GetComponent<PlayerPreferencesService>();

            uiManager.OnMenuButtonPressed += UiManager_OnMenuButtonPressed; ;
            uiManager.OnBuyPressed += UiManager_OnBuyPressed;

            balances.OnPlayerScoreChanged += Balances_OnPlayerScoreChanged;
            balances.OnEnemyScoreChanged += Balances_OnEnemyScoreChanged;
            balances.OnBalanceChanged += Balances_OnBalanceChanged;
            
            Perks.OnPerkChanged += Perks_OnPerkChanged;
                        
            DontDestroyOnLoad(this);
        }

        private void CreatePlayer()
        {
            player = Instantiate(playerPrefab).GetComponent<Player>();
            DontDestroyOnLoad(player);

            player.OnUnitBeforeKilled += Player_OnUnitBeforeKilled; ;
            player.OnWeaponSelected += Player_OnWeaponSelected;
            player.OnActiveWeaponAttack += Player_OnActiveWeaponAttack;
            player.OnWallmartApproached += Player_OnWallmartApproached;
            player.OnWallmartLeft += Player_OnWallmartLeft;

            player.Balances = Balances;
            player.Perks = Perks;
            player.Prefs = Prefs;

            player.SelectWeapon(WeaponType.NA);

            //to level start:
            InitHUD();

            player.BindInputEvents();

        }
        private void UiManager_OnBuyPressed(WallmartItem item, string playerId, PerkInfo info) =>
            uiManager.ConfirmPurchase(BuyItem(item, playerId, info));
        
        private void UiManager_OnMenuButtonPressed() =>
            StartCoroutine(LoadLobbyScene());

        private void Player_OnUnitBeforeKilled(MovableUnit obj) =>
            Balances.EnemyScore++;

        private void Perks_OnPerkChanged(PerkType arg1, int arg2)
        {
            // update HUD button (weapon icon)
            UpdateHUDPerk(arg1, arg2);

            // update markers maybe (larger shield, shield color, etc.)
            player.UpdatePerk(arg1, arg2);
        }

        private void Player_OnActiveWeaponAttack(WeaponType arg1, int arg2) =>
            uiManager.Balance.SetAmmo(arg2);

        private void Player_OnWeaponSelected(WeaponType current, WeaponType stowed)
        {
            player.InitPerkedItems();

            UpdateHUDWeapon(current, stowed);
        }



        private void Balances_OnBalanceChanged(CollectableType arg1, int arg2) =>
            UpdateHUDBalances(arg1, arg2);
        private void Balances_OnPlayerScoreChanged(int score) =>
            uiManager.UpdatePlayerScore(score, balances.CurrentCoinX);
        private void Balances_OnEnemyScoreChanged(int score) =>
            uiManager.UpdateEnemyScore(score, balances.CurrentCoinX);
        
        private void InitHUD()
        {
            uiManager.Balance.SetCoinX(Balances.CurrentCoinX);

            uiManager.Balance.SetItemAmmo(CollectableType.Bomb, Balances.CurrentBalance(CollectableType.Bomb));
            uiManager.Balance.SetItemAmmo(CollectableType.Landmine, Balances.CurrentBalance(CollectableType.Landmine));

            uiManager.Balance.SetPlayerInfo(
                Perks.CurrentPerkInfo(PlayerPerk.Shield),
                Balances.CurrentBalance(CollectableType.Coin));

            UpdateHUDWeapon(player.CurrentWeapon, player.StowedWeapon);
        }

        private void UpdateHUDWeapon(WeaponType current, WeaponType stowed)
        {
            uiManager.Balance.SetCurrentWeaponInfo(
                Perks.CurrentPerkInfo(current),
                Balances.CurrentBalance(current));
            uiManager.Balance.StowedWeapon = stowed;
            uiManager.Balance.SetStowedAmmo(Balances.CurrentBalance(stowed));
        }

        private void UpdateHUDBalances(CollectableType arg1, int arg2)
        {
            if (arg1 == CollectableType.Coin)
                uiManager.Balance.SetBalance(arg2);
            else if (PlayerBalanceService.CollectableForWeapon(player.CurrentWeapon) == arg1)
                uiManager.Balance.SetAmmo(arg2);
            else if (PlayerBalanceService.CollectableForWeapon(player.StowedWeapon) == arg1)
                uiManager.Balance.SetStowedAmmo(arg2);
            else
                uiManager.Balance.SetItemAmmo(arg1, arg2);
        }
        private void UpdateHUDPerk(PerkType perk, int level)
        {
            switch (perk)
            {
                case PerkType.Shuriken:
                    if (WeaponType.Shuriken != player.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Shuriken);
                    else
                        uiManager.Balance.SetCurrentWeaponInfo(
                            ShurikenPerks.PerkForLevel(level),
                            Balances.CurrentBalance(WeaponType.Shuriken));
                    break;
                case PerkType.Shield:
                    uiManager.Balance.SetPlayerInfo(
                        ShieldPerks.PerkForLevel(level),
                        Balances.CurrentBalance(CollectableType.Coin));
                    break;
                case PerkType.Minigun:
                    if (WeaponType.Minigun != player.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Minigun);
                    else
                        uiManager.Balance.SetCurrentWeaponInfo(
                            MinigunPerks.PerkForLevel(level),
                            Balances.CurrentBalance(WeaponType.Minigun));
                    break;
                case PerkType.Shotgun:
                    if (WeaponType.Shotgun != player.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Shotgun);
                    else
                        uiManager.Balance.SetCurrentWeaponInfo(
                            ShotgunPerks.PerkForLevel(level),
                            Balances.CurrentBalance(WeaponType.Shotgun));
                    break;
                case PerkType.Uzi:
                    if (WeaponType.Uzi != player.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Uzi);
                    else
                        uiManager.Balance.SetCurrentWeaponInfo(
                            UziPerks.PerkForLevel(level),
                            Balances.CurrentBalance(WeaponType.Uzi));
                    break;
                default:
                    break;

            };
        }

        private void Player_OnWallmartLeft() =>
            uiManager.WallmartLeft();

        private void Player_OnWallmartApproached(WallmartItem arg1, string arg2)
        {
            var money = Balances.CurrentBalance(CollectableType.Coin);
            var currentPerk = Perks.CurrentPerk(arg1);
            var perkInfo = arg1 switch
            {
                WallmartItem.Uzi => UziPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Shotgun => ShotgunPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Minigun => MinigunPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Shield => ShieldPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Shuriken => ShurikenPerks.PerkForWallmartItem(currentPerk),
                _ => default
            };
            ;
            if (!perkInfo.Equals(default) && perkInfo.WallmartItem != WallmartItem.NA)
                uiManager.WallmartApproached(perkInfo, arg2, money);
        }

        internal bool BuyItem(WallmartItem item, string playerId, PerkInfo info)
        {
            if (Balances.CurrentBalance(CollectableType.Coin) is int coins &&
                coins >= info.Price)
            {
                var level = Perks.AddPerk(item, 1);
                Balances.AddBalance(CollectableType.Coin, -info.Price, false);
                UpdateHUDPerk(PlayerPerkService.PerkForWallmart(item), level);
                return true;
            }

            return false;
        }

        private void Start() =>
            UiManager_OnMenuButtonPressed();
        
        private IEnumerator LoadLobbyScene()
        {
            var op = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);            
            op.allowSceneActivation = false;

            while (op.progress < .9f)
                yield return null;

            if (pauseBetweenLevels)
                yield return new WaitForSecondsRealtime(3.0f);

            op.allowSceneActivation = true;

            while (!op.isDone)
                yield return null;

            LobbyOp_completed(op);
        }

        private void LobbyOp_completed(AsyncOperation op)
        {
            var lobbyScene = SceneManager.GetActiveScene();
            var rootObjects = lobbyScene.GetRootGameObjects();
            var gameMenu = rootObjects.Select(x => x.GetComponent<GameMenu>()).FirstOrDefault();

            if (gameMenu != default)
                gameMenu.OnLevelSelected += GameMenu_OnLevelSelected;
        }

        private void GameMenu_OnLevelSelected(int idx)
        {
            var levelSceneName = idx switch
            {                
                4 => "Level5",
                3 => "Level4",
                2 => "Level3",
                1 => "Level2",
                _ => "Level1"
            };
            
            if (uiManager.Game)
            {
                if (player != null)
                    player.ToggleInput(false);

                uiManager.Game.CleanupLevel();
                uiManager.Game = default;
            }


            SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Single)
                .completed += (op) => {
                    AttachToScene(levelSceneName);
                };
        }

        private void AttachToScene(string levelSceneName)
        {
            Debug.Log($"Level loaded: {levelSceneName}");

            uiManager.Game = SceneManager.GetActiveScene().GetRootGameObjects()
                .Select(x => x.GetComponent<Game>())
                .Where(x => x != null)
                .FirstOrDefault();

            if (uiManager.Game != default)
            {
                if (player == null)
                    CreatePlayer();

                uiManager.Game.AttachToRunner(player, uiManager.Markers, balances);
                Debug.Log($"Level attached: {levelSceneName}");
            }
        }
    }
}