using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDScreen : MonoBehaviour
    {
        internal Action OnSettingsButtonPressed;

        [SerializeField] private Button settingsButton;

        private HUDBalance balance;

        public HUDBalance Balance => balance;

        private void Awake()
        {
            balance = GetComponent<HUDBalance>();
            settingsButton.onClick.AddListener(SettingsButtonPressed);
        }

        private void OnDestroy()
        {
            settingsButton.onClick.RemoveListener(SettingsButtonPressed);
        }

        private void SettingsButtonPressed() =>
            OnSettingsButtonPressed?.Invoke();

    }
}