using System;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDScreen : MonoBehaviour
    {
        internal Action OnSettingsButtonPressed;

        [SerializeField] private Button settingsButton;
        [SerializeField] private OnScreenStick leftStick;
        
        private void OnEnable() =>
            settingsButton.onClick.AddListener(SettingsButtonPressed);

        private void OnDisable() =>
            settingsButton.onClick.RemoveListener(SettingsButtonPressed);

        private void SettingsButtonPressed() =>
            OnSettingsButtonPressed?.Invoke();

    }
}