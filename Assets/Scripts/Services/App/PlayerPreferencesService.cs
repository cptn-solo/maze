using UnityEngine;

namespace Assets.Scripts.Services.App
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


        public const string ScoreKey = "Score"; // temporary, to keep score between games

        private void Awake()
        {
            InitPlayerPreferences();            
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

            if (!PlayerPrefs.HasKey(NickNameKey))
                PlayerPrefs.SetString(NickNameKey, "");

            if (!PlayerPrefs.HasKey(BodyTintColorKey))
                PlayerPrefs.SetString(BodyTintColorKey, BodyTintColorDefault);

            if (!PlayerPrefs.HasKey(BodyTintSliderValueKey))
                PlayerPrefs.SetFloat(BodyTintSliderValueKey, BodyTintSliderDefault);

            if (!PlayerPrefs.HasKey(ScoreKey))
                PlayerPrefs.SetInt(ScoreKey, 0);

            // PERFORMANCE
            if (!PlayerPrefs.HasKey(FpsLimitKey))
                PlayerPrefs.SetInt(FpsLimitKey, 1);

            if (!PlayerPrefs.HasKey(FpsLowTestKey))
                PlayerPrefs.SetInt(FpsLowTestKey, 0); // 0 - important

        }

    }
}
