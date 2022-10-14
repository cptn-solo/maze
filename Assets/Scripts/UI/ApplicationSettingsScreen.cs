using Assets.Scripts.Services.App;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ApplicationSettingsScreen : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button menuButton;

        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxSlider;

        [SerializeField] private Toggle fpsLimitToggle;
        [SerializeField] private Toggle fpsLowTestToggle;
        [SerializeField] private Toggle antialiasingToggle;
        
        [SerializeField] private Slider cameraSencitivitySlider;
        [SerializeField] private Toggle cameraControlToggle;


        [SerializeField] private AudioPlaybackService audioPlaybackService;
        [SerializeField] private PlayerPreferencesService playerPrefsService;

        internal Action OnCloseButtonPressed;
        internal Action OnMenuButtonPressed;

        void Start()
        {
            musicToggle.isOn = audioPlaybackService.MusicToggle;
            musicSlider.value = audioPlaybackService.MusicVolume;

            sfxToggle.isOn = audioPlaybackService.SfxToggle;
            sfxSlider.value = audioPlaybackService.SfxVolume;

            cameraSencitivitySlider.value = playerPrefsService.CameraSencitivity;
            cameraControlToggle.isOn = playerPrefsService.CameraControl;

            fpsLimitToggle.isOn = FpsLimitToggle;
            fpsLowTestToggle.isOn = FpsLowTestToggle;
            antialiasingToggle.isOn = Antialiasing2xToggle;
        }

        public void Close() => OnCloseButtonPressed?.Invoke();
        public void Menu() => OnMenuButtonPressed?.Invoke();

        private void Awake()
        {
            fpsLimitToggle.onValueChanged.AddListener(OnFpsLimitToggleChange);
            fpsLowTestToggle.onValueChanged.AddListener(OnFpsLowTestToggleChange);

            musicToggle.onValueChanged.AddListener(OnMusicToggleChange);
            musicSlider.onValueChanged.AddListener(OnMusicSliderChange);

            sfxToggle.onValueChanged.AddListener(OnSfxToggleChange);
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChange);

            antialiasingToggle.onValueChanged.AddListener(OnAntialiasing2xToggleChange);

            closeButton.onClick.AddListener(Close);
            menuButton.onClick.AddListener(Menu);

            cameraSencitivitySlider.onValueChanged.AddListener(OnCameraSencitivityChange);
            cameraControlToggle.onValueChanged.AddListener(OnCameraControlChange);
        }

        private void OnDestroy()
        {
            fpsLimitToggle.onValueChanged.RemoveListener(OnFpsLimitToggleChange);
            fpsLowTestToggle.onValueChanged.RemoveListener(OnFpsLowTestToggleChange);

            musicToggle.onValueChanged.RemoveListener(OnMusicToggleChange);
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChange);

            sfxToggle.onValueChanged.RemoveListener(OnSfxToggleChange);
            sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChange);

            antialiasingToggle.onValueChanged.RemoveListener(OnAntialiasing2xToggleChange);

            cameraSencitivitySlider.onValueChanged.RemoveListener(OnCameraSencitivityChange);
            cameraControlToggle.onValueChanged.RemoveListener(OnCameraControlChange);

            closeButton.onClick.RemoveListener(Close);
            menuButton.onClick.RemoveListener(Menu);
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
        public bool Antialiasing2xToggle
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.Antialiasing2xKey) != 0;
            set => PlayerPrefs.SetInt(PlayerPreferencesService.Antialiasing2xKey, value ? 1 : 0);
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

        public void OnAntialiasing2xToggleChange(bool value)
        {
            QualitySettings.antiAliasing = value ? 2 : 0;

            Antialiasing2xToggle = antialiasingToggle.isOn;

        }

        public void OnCameraSencitivityChange(float value)
        {
            playerPrefsService.CameraSencitivity = value;
        }
        public void OnCameraControlChange(bool value)
        {
            playerPrefsService.CameraControl = value;
        }

    }
}