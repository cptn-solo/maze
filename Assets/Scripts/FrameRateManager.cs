using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FrameRateManager : MonoBehaviour
{
    [Header("Frame Settings")]
    public int MaxRate = 9999;
    public float TargetFrameRate = 60.0f;
    float currentFrameTime;
    
    public int avgFrameRate;
    public string display_Text;
    GUIStyle textStyle = new GUIStyle();

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = 0.02f;
        if (Application.isMobilePlatform)
        {
            QualitySettings.vSyncCount = 2;
            TargetFrameRate = 20.0f;
            MaxRate = 20;
            Time.fixedDeltaTime = 0.05f;
        }


        Application.targetFrameRate = MaxRate;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine("WaitForNextFrame");
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

    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / TargetFrameRate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(55, 55, 100, 25), display_Text, textStyle);
    }
}