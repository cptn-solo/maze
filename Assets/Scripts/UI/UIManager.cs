using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private SettingsScreen SettingsScreen;
        [SerializeField] private HUDScreen HUDScreen;
        [SerializeField] private HUDMarkersView Markers;
        [SerializeField] private HUDLeaderBoardView Leaderboard;

        private void OnEnable()
        {
            HUDScreen.OnSettingsButtonPressed += ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed += CloseSettingsScreen;
        }
        private void OnDisable()
        {
            HUDScreen.OnSettingsButtonPressed -= ShowSettingsScreen;
            SettingsScreen.OnCloseButtonPressed -= CloseSettingsScreen;
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