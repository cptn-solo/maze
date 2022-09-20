using Assets.Scripts.Services.App;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ApplicationSettingsScreen : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxSlider;

        [SerializeField] private Toggle fpsLimitToggle;
        [SerializeField] private Toggle fpsLowTestToggle;


        [SerializeField] private AudioPlaybackService audioPlaybackService;

        internal Action OnCloseButtonPressed;

        void Start()
        {
            musicToggle.isOn = audioPlaybackService.MusicToggle;
            musicSlider.value = audioPlaybackService.MusicVolume;

            sfxToggle.isOn = audioPlaybackService.SfxToggle;
            sfxSlider.value = audioPlaybackService.SfxVolume;

            fpsLimitToggle.isOn = FpsLimitToggle;
            fpsLowTestToggle.isOn = FpsLowTestToggle;
        }

        public void Close() => OnCloseButtonPressed?.Invoke();

        private void OnEnable()
        {
            fpsLimitToggle.onValueChanged.AddListener(OnFpsLimitToggleChange);
            fpsLowTestToggle.onValueChanged.AddListener(OnFpsLowTestToggleChange);

            musicToggle.onValueChanged.AddListener(OnMusicToggleChange);
            musicSlider.onValueChanged.AddListener(OnMusicSliderChange);

            sfxToggle.onValueChanged.AddListener(OnSfxToggleChange);
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChange);

            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            fpsLimitToggle.onValueChanged.RemoveListener(OnFpsLimitToggleChange);
            fpsLowTestToggle.onValueChanged.RemoveListener(OnFpsLowTestToggleChange);

            musicToggle.onValueChanged.RemoveListener(OnMusicToggleChange);
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChange);

            sfxToggle.onValueChanged.RemoveListener(OnSfxToggleChange);
            sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChange);

            closeButton.onClick.RemoveListener(Close);
        }
        
        public bool FpsLimitToggle
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.FpsLimitKey) != 0;
            set => PlayerPrefs.SetInt(PlayerPreferencesService.FpsLimitKey, value ? 1 : 0);
        }
        public bool FpsLowTestToggle
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.FpsLowTestKey) != 0;
            set => PlayerPrefs.SetInt(PlayerPreferencesService.FpsLowTestKey, value ? 1 : 0);
        }

        public void OnFpsLimitToggleChange(bool value)
        {
            FpsLimitToggle = value;
        }
        public void OnFpsLowTestToggleChange(bool value)
        {
            FpsLowTestToggle = value;
        }

        public void OnMusicSliderChange(float value)
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.MusicVolume = (int)musicSlider.value;
        }

        public void OnSfxSliderChange(float value)
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.SfxVolume = (int)sfxSlider.value;
        } 

        public void OnMusicToggleChange(bool value)
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.MusicToggle = musicToggle.isOn;
        }

        public void OnSfxToggleChange(bool value)
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.SfxToggle = sfxToggle.isOn;
        }
    }
}