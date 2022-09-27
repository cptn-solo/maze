using System;

namespace Assets.Scripts
{
    public partial class Player
    {
        public event Action<WallmartItem, string> OnWallmartApproached;
        public event Action OnWallmartLeft;

        internal void ShowWallmart(WallmartItem itemType)
        {
            OnWallmartApproached?.Invoke(itemType, hitbox.PlayerId);
        }

        internal void HideWallmart()
        {
            OnWallmartLeft();
        }

    }
}