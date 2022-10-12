using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ApplicationSettingsScreen SettingsScreen;
        [SerializeField] private HUDScreen HUDScreen;
        [SerializeField] private WallmartScreen WallmartScreen;
        [SerializeField] private HUDMarkersView Markers;
        [SerializeField] private HUDLeaderBoardView Leaderboard;

        [SerializeField] private Game game;

        private InputSystemUIInputModule inputModule;

        public GameRunner GameRunner { get; set; }

        private void Awake()
        {
            inputModule = GetComponent<InputSystemUIInputModule>();
        }

        private void OnEnable()
        {
            HUDScreen.OnSettingsButtonPressed += ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed += CloseSettingsScreen;
            SettingsScreen.OnMenuButtonPressed += ShowGameMenu;
            WallmartScreen.OnBuyPressed += BuyWallmartItem;
            WallmartScreen.OnCancelPressed += Game_OnWallmartLeft;

            game.OnPlayerKilled += Game_OnPlayerKilled;
            game.OnPlayerSpawned += Game_OnPlayerSpawned;
            game.OnWallmartLeft += Game_OnWallmartLeft;
            game.OnWallmartApproached += Game_OnWallmartApproached;
        }

        private void Game_OnWallmartApproached(PerkInfo e, string playerId, int playerBalance)
        {
            WallmartScreen.gameObject.SetActive(true);
            WallmartScreen.ShowItemCard(e, playerId, playerBalance);
        }

        private void Game_OnWallmartLeft() =>
            WallmartScreen.gameObject.SetActive(false);
        
        private void Game_OnPlayerSpawned(object sender, System.EventArgs e)
        {
            HUDScreen.gameObject.SetActive(true);
            inputModule.enabled = true;
        }

        private void Game_OnPlayerKilled(object sender, System.EventArgs e)
        {
            inputModule.enabled = false;
            HUDScreen.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            HUDScreen.OnSettingsButtonPressed -= ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed -= CloseSettingsScreen;
            SettingsScreen.OnMenuButtonPressed -= ShowGameMenu;
            WallmartScreen.OnBuyPressed -= BuyWallmartItem;
            WallmartScreen.OnCancelPressed -= Game_OnWallmartLeft;


            game.OnPlayerKilled -= Game_OnPlayerKilled;
            game.OnPlayerSpawned -= Game_OnPlayerSpawned;
            game.OnWallmartLeft -= Game_OnWallmartLeft;
            game.OnWallmartApproached -= Game_OnWallmartApproached;
        }

        private void BuyWallmartItem(WallmartItem item, string playerId, PerkInfo info)
        {
            var success = game.BuyItem(item, playerId, info);
            WallmartScreen.CompletePurchase(success);
        }


        private void ShowSettingsScreen()
        {
            inputModule.enabled = false;
            HUDScreen.gameObject.SetActive(false);
            SettingsScreen.gameObject.SetActive(true);
            inputModule.enabled = true;
        }

        private void CloseSettingsScreen()
        {
            inputModule.enabled = false;
            HUDScreen.gameObject.SetActive(true);
            SettingsScreen.gameObject.SetActive(false);
            inputModule.enabled = true;
        }

        private void ShowGameMenu()
        {
            game.CleanupLevel();
            GameRunner.LoadLobby();
        }
    }
}