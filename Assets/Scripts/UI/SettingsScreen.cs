using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SettingsScreen : MonoBehaviour
    {
        public const string FpsLimitKey = "FpsLimitKey";
        public const string FpsLowTestKey = "FpsLowTestKey";

        [SerializeField] private Toggle fpsLimitToggle;
        [SerializeField] private Toggle fpsLowTestToggle;
        [SerializeField] private Button closeButton;
        
        internal Action OnCloseButtonPressed;

        public void Start()
        {
            if (!PlayerPrefs.HasKey(FpsLimitKey))
                PlayerPrefs.SetInt(FpsLimitKey, 1);

            if (!PlayerPrefs.HasKey(FpsLowTestKey))
                PlayerPrefs.SetInt(FpsLowTestKey, 0); // 0 - important

            fpsLimitToggle.isOn = FpsLimitToggle;
            fpsLowTestToggle.isOn = FpsLowTestToggle;
        }
        private void OnEnable()
        {
            fpsLimitToggle.onValueChanged.AddListener(OnFpsLimitToggleChange);
            fpsLowTestToggle.onValueChanged.AddListener(OnFpsLowTestToggleChange);

            closeButton.onClick.AddListener(Close);
        }
        private void OnDisable()
        {
            fpsLimitToggle.onValueChanged.RemoveListener(OnFpsLimitToggleChange);
            fpsLowTestToggle.onValueChanged.RemoveListener(OnFpsLowTestToggleChange);
            
            closeButton.onClick.RemoveListener(Close);
        }

        public bool FpsLimitToggle
        {
            get => PlayerPrefs.GetInt(FpsLimitKey) != 0;
            set => PlayerPrefs.SetInt(FpsLimitKey, value ? 1 : 0);
        }
        public bool FpsLowTestToggle
        {
            get => PlayerPrefs.GetInt(FpsLowTestKey) != 0;
            set => PlayerPrefs.SetInt(FpsLowTestKey, value ? 1 : 0);
        }

        public void OnFpsLimitToggleChange(bool value)
        {
            FpsLimitToggle = value;
        }
        public void OnFpsLowTestToggleChange(bool value)
        {
            FpsLowTestToggle = value;
        }

        public void Close() => OnCloseButtonPressed?.Invoke();
    }
}