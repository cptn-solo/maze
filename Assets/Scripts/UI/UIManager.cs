using UnityEngine;

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

        private void OnEnable()
        {
            HUDScreen.OnSettingsButtonPressed += ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed += CloseSettingsScreen;
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
        
        private void Game_OnPlayerSpawned(object sender, System.EventArgs e) =>
            HUDScreen.gameObject.SetActive(true);

        private void Game_OnPlayerKilled(object sender, System.EventArgs e) =>
            HUDScreen.gameObject.SetActive(false);

        private void OnDisable()
        {
            HUDScreen.OnSettingsButtonPressed -= ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed -= CloseSettingsScreen;
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
            HUDScreen.gameObject.SetActive(false);
            SettingsScreen.gameObject.SetActive(true);
        }

        private void CloseSettingsScreen()
        {
            HUDScreen.gameObject.SetActive(true);
            SettingsScreen.gameObject.SetActive(false);
        }
    }
}