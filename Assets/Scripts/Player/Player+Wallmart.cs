using System;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts
{
    public partial class Player
    {
        public event Action<WallmartItem, string> OnWallmartApproached;
        public event Action<int, string> OnLevelWallmartApproached;
        public event Action OnWallmartLeft;

        internal void ShowWallmart(WallmartItem itemType) =>
            OnWallmartApproached?.Invoke(itemType, hitbox.PlayerId);

        internal void ShowLevelWallmart(int levelId) =>
            OnLevelWallmartApproached?.Invoke(levelId, hitbox.PlayerId);

        internal void HideWallmart() =>
            OnWallmartLeft();

    }
}