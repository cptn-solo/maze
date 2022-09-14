namespace Assets.Scripts
{
    public struct FrameRatePreset
    {
        public int TargetFrameRate;
        public int vSyncCount;
        public float FixedDeltaTime;

        public bool OnlyTargetFrameRate;

        public static FrameRatePreset Default()
        {
            FrameRatePreset preset = default;

            preset.TargetFrameRate = 288;
            preset.vSyncCount = 0;
            preset.FixedDeltaTime = 0.02f;

            return preset;
        }

        public static FrameRatePreset Mobile()
        {
            FrameRatePreset preset = default;

            preset.TargetFrameRate = 30;
            preset.vSyncCount = 2;
            preset.FixedDeltaTime = 0.0334f;

            return preset;
        }

        public static FrameRatePreset Low()
        {
            FrameRatePreset preset = default;

            preset.TargetFrameRate = 20;
            preset.vSyncCount = 2;
            preset.FixedDeltaTime = 0.05f;

            return preset;
        }


    }
}