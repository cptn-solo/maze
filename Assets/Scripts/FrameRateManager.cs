using System.Collections;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts
{
    public class FrameRateManager : MonoBehaviour
    {
        [Header("Frame Settings")]
        float currentFrameTime;

        public int avgFrameRate;
        public string display_Text;
        
        readonly GUIStyle textStyle = new();
        
        private FrameRatePreset preset;

        void Awake()
        {

            preset = FrameRatePreset.Default();

            if (Application.isMobilePlatform)
                preset = FrameRatePreset.Mobile();

            var prefLimit = PlayerPrefs.GetInt(SettingsScreen.FpsLimitKey) != 0;

            if (prefLimit)
                preset = FrameRatePreset.Mobile();

            var prefLowTest = PlayerPrefs.GetInt(SettingsScreen.FpsLowTestKey) != 0;

            if (prefLowTest)
                preset = FrameRatePreset.Low();

            preset.OnlyTargetFrameRate = true;

            if (Application.isMobilePlatform && prefLimit)
                preset.OnlyTargetFrameRate = false;

            Time.fixedDeltaTime = preset.FixedDeltaTime;
            QualitySettings.vSyncCount = preset.vSyncCount;

            Application.targetFrameRate = preset.TargetFrameRate;
            
            currentFrameTime = Time.realtimeSinceStartup;
            _ = StartCoroutine(nameof(WaitForNextFrame));
        }

        IEnumerator WaitForNextFrame()
        {
            while (true)
            {
                if (preset.OnlyTargetFrameRate)
                {
                    Application.targetFrameRate = preset.TargetFrameRate;
                    yield return null;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                    currentFrameTime += 1.0f / preset.TargetFrameRate;
                    var t = Time.realtimeSinceStartup;
                    var sleepTime = currentFrameTime - t - 0.01f;
                    if (sleepTime > 0)
                        Thread.Sleep((int)(sleepTime * 1000));
                    while (t < currentFrameTime)
                        t = Time.realtimeSinceStartup;
                }
            }
        }

        private void Start()
        {
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.normal.textColor = Color.white;
        }

        public void Update()
        {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            avgFrameRate = (int)current;
            display_Text = avgFrameRate.ToString() + " FPS";
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(55, 55, 100, 25), display_Text, textStyle);
        }
    }
}