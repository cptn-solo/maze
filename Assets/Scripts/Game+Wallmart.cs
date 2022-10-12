using System;

namespace Assets.Scripts
{
    public partial class Game
    {
        public event Action<PerkInfo, string, int> OnWallmartApproached;
        public event Action OnWallmartLeft;
        private void Player_OnWallmartLeft()
        {
            OnWallmartLeft?.Invoke();
        }

        private void Player_OnWallmartApproached(WallmartItem arg1, string arg2)
        {
            var money = balances.CurrentBalance(CollectableType.Coin);
            var currentPerk = perks.CurrentPerk(arg1);
            var perkInfo = arg1 switch
            {
                WallmartItem.Uzi => UziPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Shotgun => ShotgunPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Minigun => MinigunPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Shield => ShieldPerks.PerkForWallmartItem(currentPerk),
                WallmartItem.Shuriken => ShurikenPerks.PerkForWallmartItem(currentPerk),
                _ => default
            };
                ;
            if (!perkInfo.Equals(default))
                OnWallmartApproached?.Invoke(perkInfo, arg2, money);
        }

        internal bool BuyItem(WallmartItem item, string playerId, PerkInfo info)
        {
            if (balances.CurrentBalance(CollectableType.Coin) is int coins && 
                coins >= info.Price)
            {
                var level = perks.AddPerk(item, 1);
                balances.AddBalance(CollectableType.Coin, -info.Price, false);
                UpdateHUDPerk(PlayerPerkService.PerkForWallmart(item), level);
                return true;
            }

            return false;
        }
    }
}