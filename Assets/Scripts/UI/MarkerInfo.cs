using UnityEngine;

namespace Assets.Scripts.UI
{
    public struct MarkerInfo
    {
        public string Label;
        public Color LabelColor;
        public string Info;

        public static MarkerInfo Get(string label, Color color, string info)
        {
            MarkerInfo marker = default;
            marker.Label = label;
            marker.LabelColor = color;
            marker.Info = info;

            return marker;
        }

        public void SetInfo(string label, Color color, string info)
        {
            Label = label;
            LabelColor = color;
            Info = info;
        }
    }
}