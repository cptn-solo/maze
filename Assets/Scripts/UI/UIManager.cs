using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ApplicationSettingsScreen SettingsScreen;
        [SerializeField] private HUDScreen HUDScreen;
        [SerializeField] private WallmartScreen WallmartScreen;
        [SerializeField] private HUDMarkersView markers;
        [SerializeField] private HUDLeaderBoardView Leaderboard;
        [SerializeField] private HUDBalance balance;
        public HUDMarkersView Markers => markers;

        public HUDBalance Balance => balance;

        public event Action OnMenuButtonPressed;
        public event Action<WallmartItem, string, PerkInfo> OnBuyPressed;

        private Game game;

        public Game Game { 
            get => game; 
            set
            {
                if (value != default)
                {
                    game = value;

                    game.OnPlayerKilled += Game_OnPlayerKilled;
                    game.OnPlayerSpawned += Game_OnPlayerSpawned;
                }
                else if (game != default)
                {
                    game.OnPlayerKilled -= Game_OnPlayerKilled;
                    game.OnPlayerSpawned -= Game_OnPlayerSpawned;

                    game = default;
                }
            } 
        }

        private void Awake()
        {
            HUDScreen.OnSettingsButtonPressed += ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed += CloseSettingsScreen;
            SettingsScreen.OnMenuButtonPressed += ShowGameMenu;
            WallmartScreen.OnBuyPressed += BuyWallmartItem;
            WallmartScreen.OnCancelPressed += WallmartLeft;

            DontDestroyOnLoad(this);
        }
        private void OnDestroy()
        {
            HUDScreen.OnSettingsButtonPressed -= ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed -= CloseSettingsScreen;
            SettingsScreen.OnMenuButtonPressed -= ShowGameMenu;
            WallmartScreen.OnBuyPressed -= BuyWallmartItem;
            WallmartScreen.OnCancelPressed -= WallmartLeft;
        }

        private void Game_OnPlayerSpawned(object sender, System.EventArgs e)
        {
            HUDScreen.gameObject.SetActive(true);
            Leaderboard.gameObject.SetActive(true);
            Markers.gameObject.SetActive(true);
        }

        private void Game_OnPlayerKilled(object sender, System.EventArgs e)
        {
            HUDScreen.gameObject.SetActive(false);
            Leaderboard.gameObject.SetActive(false);
            Markers.gameObject.SetActive(false);
        }

        private void BuyWallmartItem(WallmartItem item, string playerId, PerkInfo info) =>
            OnBuyPressed?.Invoke(item, playerId, info);

        internal void ConfirmPurchase(bool success) =>
            WallmartScreen.CompletePurchase(success);

        private void ShowSettingsScreen()
        {
            HUDScreen.gameObject.SetActive(false);
            Leaderboard.gameObject.SetActive(false);
            Markers.gameObject.SetActive(false);

            SettingsScreen.gameObject.SetActive(true);
        }

        private void CloseSettingsScreen()
        {
            HUDScreen.gameObject.SetActive(true);
            Leaderboard.gameObject.SetActive(true);
            Markers.gameObject.SetActive(true);

            SettingsScreen.gameObject.SetActive(false);
        }

        private void ShowGameMenu()
        {
            Game.CleanupLevel();
            OnMenuButtonPressed?.Invoke();
            SettingsScreen.gameObject.SetActive(false);
        }
        internal void WallmartApproached(PerkInfo e, string playerId, int playerBalance)
        {
            WallmartScreen.gameObject.SetActive(true);
            WallmartScreen.ShowItemCard(e, playerId, playerBalance);
        }

        internal void WallmartLeft() =>
            WallmartScreen.gameObject.SetActive(false);

        internal void UpdatePlayerScore(int score, int coinX)
        {
            Leaderboard.UpdatePlayer(score);
            HUDScreen.Balance.SetCoinX(coinX);
        }
        internal void UpdateEnemyScore(int score, int coinX)
        {
            Leaderboard.UpdateEnemy(score);
            HUDScreen.Balance.SetCoinX(coinX);
        }

        internal void UpdatePlayerAmmo(int ammo) =>
            HUDScreen.Balance.SetAmmo(ammo);

    }
}