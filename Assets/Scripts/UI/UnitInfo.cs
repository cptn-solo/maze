using UnityEngine;

namespace Assets.Scripts.UI
{
    public struct UnitInfo
    {
        public string NickName;
        public Color BodyTintColor;
        public int Score;
        public Color ScoreColor;

        public UnitInfo(string nickName, Color bodyTintColor, int score, Color scoreColor)
        {
            NickName = nickName;
            BodyTintColor = bodyTintColor;
            Score = score;
            ScoreColor = scoreColor;
        }
        public string Serialize()
        {
            var colorHex = HexStringFromColor(BodyTintColor);
            var scoreColorHex = HexStringFromColor(ScoreColor);

            return $"{NickName}:{colorHex}:{Score}:{scoreColorHex}";
        }

        public static UnitInfo Deserialize(string playerInfo)
        {
            Debug.Log($"PlayerInfo Deserialize {playerInfo}");

            var props = playerInfo.Split(":");

            UnitInfo info = default;
            info.NickName = props[0];
            info.BodyTintColor = ColorFromHexString(props[1]);
            info.Score = int.Parse(props[2]);
            info.ScoreColor = ColorFromHexString(props[3]);
            return info;
        }
        public static string HexStringFromColor(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
        public static Color ColorFromHexString(string hexString)
        {
            if (ColorUtility.TryParseHtmlString($"#{hexString}", out var color))
                return color;
            return Color.black;
        }
    }
}