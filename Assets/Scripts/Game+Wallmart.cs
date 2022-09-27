using System;

namespace Assets.Scripts
{
    public partial class Game
    {
        public event Action<WallmartItem, string, int> OnWallmartApproached;
        public event Action OnWallmartLeft;
        private void Player_OnWallmartLeft()
        {
            OnWallmartLeft?.Invoke();
        }

        private void Player_OnWallmartApproached(WallmartItem arg1, string arg2)
        {
            var money = balances.CurrentBalance(CollectableType.Coin);
            OnWallmartApproached?.Invoke(arg1, arg2, money);
        }

        internal bool BuyItem(WallmartItem item, string playerId, int price)
        {
            if (balances.CurrentBalance(CollectableType.Coin) is int coins && 
                coins >= price)
            {
                perks.AddPerk(item, 1);
                balances.AddBalance(CollectableType.Coin, -price);
                UpdateHUDPerk(PlayerPerkService.PerkForWallmart(item));
                return true;
            }

            return false;
        }
    }
}