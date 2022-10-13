using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerPreferencesService : MonoBehaviour
    {
        public const string MusicToggleKey = "MusicToggle";
        public const string SfxToggleKey = "SfxToggle";
        public const string MusicVolumeKey = "MusicVolume";
        public const int MusicVolumeDefault = -20;

        public const string SfxVolumeKey = "SfxVolume";
        public const int SfxVolumeDefault = 0;

        public const string NickNameKey = "NickName";
        public const string BodyTintColorKey = "BodyTintColor";
        public const string BodyTintColorDefault = "E72400";
        public const string BodyTintSliderValueKey = "BodyTintSliderValue";
        public const float BodyTintSliderDefault = 0f;

        public const string FpsLimitKey = "FpsLimitKey";
        public const string FpsLowTestKey = "FpsLowTestKey";
        public const string Antialiasing2xKey = "Antialiasing2xKey";
        public const string CameraControlKey = "CameraControlKey";
        public const string CameraSencitivityKey = "CameraSencitivityKey";
        public const float CameraSencitivityDefault = .1f;

        public const string ScoreKey = "Score"; // temporary, to keep score between games
        public const string BalanceKey = "Balance"; // temporary, to keep score between games        
        public const string WeaponKey = "Weapon"; // temporary, to keep score between games        
        public const string PerkKey = "Perk"; // temporary, to keep score between games        

        public event Action<float> OnCameraSencitivityChanged;
        public event Action<bool> OnCameraControlChanged;

        private void Awake()
        {
            InitPlayerPreferences();
#if UNITY_EDITOR
            InitDeveloperPreferences();
#endif
        }

        public float CameraSencitivity
        {
            get => PlayerPrefs.GetFloat(PlayerPreferencesService.CameraSencitivityKey);
            set
            {
                PlayerPrefs.SetFloat(PlayerPreferencesService.CameraSencitivityKey, value);
                OnCameraSencitivityChanged?.Invoke(value);
            }
        }
        public bool CameraControl
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.CameraControlKey) == 1;
            set
            {
                PlayerPrefs.SetInt(PlayerPreferencesService.CameraControlKey, value ? 1 : 0);
                OnCameraControlChanged?.Invoke(value);
            }
        }


        public void InitPlayerPreferences()
        {
            if (!PlayerPrefs.HasKey(MusicToggleKey))
                PlayerPrefs.SetInt(MusicToggleKey, 1);
            
            if (!PlayerPrefs.HasKey(SfxToggleKey))
                PlayerPrefs.SetInt(SfxToggleKey, 1);

            if (!PlayerPrefs.HasKey(MusicVolumeKey))
                PlayerPrefs.SetInt(MusicVolumeKey, MusicVolumeDefault);

            if (!PlayerPrefs.HasKey(SfxVolumeKey))
                PlayerPrefs.SetInt(SfxVolumeKey, SfxVolumeDefault);
            
            if (!PlayerPrefs.HasKey(CameraControlKey))
                PlayerPrefs.SetInt(CameraControlKey, 1);

            if (!PlayerPrefs.HasKey(CameraSencitivityKey))
                PlayerPrefs.SetFloat(CameraSencitivityKey, CameraSencitivityDefault);

            if (!PlayerPrefs.HasKey(NickNameKey))
                PlayerPrefs.SetString(NickNameKey, "");

            if (!PlayerPrefs.HasKey(BodyTintColorKey))
                PlayerPrefs.SetString(BodyTintColorKey, BodyTintColorDefault);

            if (!PlayerPrefs.HasKey(BodyTintSliderValueKey))
                PlayerPrefs.SetFloat(BodyTintSliderValueKey, BodyTintSliderDefault);

            if (!PlayerPrefs.HasKey(ScoreKey))
                PlayerPrefs.SetInt(ScoreKey, 0);

            foreach (var collectable in new[] {
                CollectableType.Coin,
                CollectableType.Minigun,
                CollectableType.Bomb,
                CollectableType.Landmine,
            })
            {
                var key = PlayerBalanceService.BalanceKey(collectable);
                var hasKey = PlayerPrefs.HasKey(key);
                if (!hasKey)
                    PlayerPrefs.SetInt(key, 0);
            }

            // Most perks initialized as locked:
            foreach (var perk in new[] {
                PerkType.NA,
                PerkType.Power,
                PerkType.Minigun,
                PerkType.Shotgun,
                PerkType.Uzi,
            })
            {
                var key = PlayerPerkService.PerkKey(perk);
                var hasKey = PlayerPrefs.HasKey(key);
                if (!hasKey)
                    PlayerPrefs.SetInt(key, 0);
            }

            // Some perks are initially unlocked:
            foreach (var perk in new[] {
                PerkType.Shield,
                PerkType.Shuriken,
            })
            {
                var key = PlayerPerkService.PerkKey(perk);
                var hasKey = PlayerPrefs.HasKey(key);
                if (!hasKey)
                    PlayerPrefs.SetInt(key, 1);
            }

            // PERFORMANCE
            if (!PlayerPrefs.HasKey(FpsLimitKey))
                PlayerPrefs.SetInt(FpsLimitKey, 1);

            if (!PlayerPrefs.HasKey(FpsLowTestKey))
                PlayerPrefs.SetInt(FpsLowTestKey, 0); // 0 - important

            if (!PlayerPrefs.HasKey(Antialiasing2xKey))
                PlayerPrefs.SetInt(Antialiasing2xKey, 0); // 0 - important
        }

        public void InitDeveloperPreferences()
        {
            foreach (var collectable in new[] {
                CollectableType.Coin,
                CollectableType.Minigun,
                CollectableType.Bomb,
                CollectableType.Landmine,
            })
            {
                var key = PlayerBalanceService.BalanceKey(collectable);
                PlayerPrefs.SetInt(key, 5001);
            }

            // Most perks initialized as locked:
            foreach (var perk in new[] {
                PerkType.NA,
                PerkType.Power,
                PerkType.Uzi,
                PerkType.Shotgun,
                PerkType.Minigun,
            })
            {
                var key = PlayerPerkService.PerkKey(perk);
                PlayerPrefs.SetInt(key, 0);
            }

            // Some perks are initially unlocked:
            foreach (var perk in new[] {
                PerkType.Shield,
                PerkType.Shuriken,
            })
            {
                var key = PlayerPerkService.PerkKey(perk);
                PlayerPrefs.SetInt(key, 1);
            }

        }
    }
}
