using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDScreen : MonoBehaviour
{
    internal Action OnSettingsButtonPressed;

    [SerializeField] private Button settingsButton;

    private void OnEnable()
    {
        settingsButton.onClick.AddListener(SettingsButtonPressed);
    }

    private void OnDisable()
    {
        settingsButton.onClick.RemoveListener(SettingsButtonPressed);
    }

    private void SettingsButtonPressed()
    {
        OnSettingsButtonPressed?.Invoke();
    }
}
